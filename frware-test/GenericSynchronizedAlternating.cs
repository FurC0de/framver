using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frware_test
{
    /*
     * Lets figure it out.
     * Thread A is a producer.
     * Thread B is a consumer.
     * 
     * Thread A can generate values in any rate, meaning these modifications can't be synced up.
     * Thread B should be able to recieve values at any point in time to avoid "stuttering". This thread is very time-sensitive.
     * 
     * From the start:
     * 
     * ...
     * Mod | Lock | Function  |
     *   - |      | Modify    | Thread A asks for a non-locked value to modify and gets the Front value
     *   - | F    | _lock     | Thread A marks that the Front value is currently locked
     *   - | F    | <-        | Thread A writes to the Front value
     *
     *   - | F    | Read      | Meanwhile, Thread B at the same time asks for a non-locked modified value
     *   - | F    |           | Thread B gets the Back value because the Front value is locked and there is no modified values yet
     *   - | F  B | _lock     | Thread B locks the Back value
     *   - | F  B | <-        | Thread B reads the Back value (considered as initial value)
     *   - | F  B | _unlock   | Thread B unlocks the Back value
     *
     *   - | F    | _unlock   | Thread A finishes writing to the Front value and unlocks it
     *   F | F    | _modified | Thread A marks that the Front value was modified
     *
     *   F |      | Read      | Thread B asks for a non-locked modified value to read
     *   F |      |           | Thread B gets the Front value as it is modified and non-locked
     *   F | F    | _lock     | Thread B locks the Front value
     *   F | F    | _unlock   | Thread B reads the Front value
     *
     *   F | F    |           | Thread A asks for a non-locked value to modify and gets the Back value 
     *   F | F  B | _lock     | Thread A marks that the Back value is currently locked
     *   F | F  B | <-        | Thread A writes to the Back value
     *
     *   F | F  B | _unlock   | Thread B unlocks the Front value
     *
     *   F |    B | _unlock   | Thread A finishes writing to the Back value and unlocks it
     *   B |    B | _modified | Thread A marks that the Front value was modified
     *
     *   B |      | Read      | Thread B asks for a non-locked modified value to read
     *   B |      |           | Thread B gets the Back value as it is modified and non-locked
     *   B |    B | _lock     | Thread B locks the Back value
     *   B |    B | <-        | Thread B reads the Back value
     *   B |    B | _unlock   | Thread B unlocks the Front value
     * ...
     * 
     * States.
     * 
     * Mod:
     * 0 - no modded values
     * 1 - F is modded
     * 2 - B is modded
     * 
     * Lock
     * 0 - no locked values
     * 1 - F is locked
     * 2 - B is locked
     * 3 - F&B are locked
     * 
     * 0b00000000 00000000 00000000 00000011 (0b11) is a lock mask
     * 0b00000000 00000000 00000000 00001100 (0b1100) is a mod mask
     * 
     * !!! THERE MIGHT BE CATCH-UP FAILURE CONDITION (NOT RACING CONDITION) !!!
     * !!!                                                                  !!!
     * !!! When producer thread will lock one value at the same time as co- !!!
     * !!! nsumer thread wants to get last modded value. If it happens in   !!!
     * !!! sync more than once there would be a possible lag. Adding a cou- !!!
     * !!! nter that allows us to check how long ago each value was updated !!!
     * !!! may help.                                                        !!!
     * !!! This issue is critical and should be fixed.                      !!!
     */

    internal class GenericSynchronizedAlternating<T>
    {
        private T? ValueFront;

        private T? ValueBack;

        private uint _state = 0; // 0x00000000

        public GenericSynchronizedAlternating(object[] parameters)
        {
            ValueFront = (T?)Activator.CreateInstance(typeof(T), parameters);
            ValueBack = (T?)Activator.CreateInstance(typeof(T), parameters);
        }

        public GenericSynchronizedAlternating(T front, T back)
        {
            ValueFront = front;
            ValueBack = back;
        }

        public (T?, uint) Modify()
        {
            // Check if Front is locked.
            // If not - select Front as used value,
            // Else - select Back as used value.
            uint usedValue = (uint)(GetLck() != 1 ? 1 : 2);
            Lock(usedValue);

            return (usedValue == 1 ? ValueFront : ValueBack, usedValue);
        }

        public (T?, uint) Read() 
        {
            uint usedValue;
            switch (GetMod())
            {
                case 0: 
                    usedValue = GetLck() == 1 ? (uint)2 : (uint)1;
                    break;
                case 1:
                    usedValue = GetLck() == 1 ? (uint)2 : (uint)1;
                    break;
                case 2:
                    usedValue = GetLck() == 2 ? (uint)1 : (uint)2;
                    break;
                default: 
                    throw new ArgumentOutOfRangeException();
            }

            Lock(usedValue);

            return (usedValue == 1 ? ValueFront : ValueBack, usedValue);
        }

        public void Lock(uint lck) {
            SetStateLck(lck);
        }
        public void Unlock(uint lck) {
            UnsetStateLck(lck);
        }

        private void SetStateMod(uint mod)
        {
            SpinWait spin = new SpinWait();
            while (true)
            {
                uint init = _state;
                uint val = (mod << 2) & 0b1100 | init & 0b11;
                if (Interlocked.CompareExchange(ref _state, val, init) == init)
                {
                    break;
                }
                spin.SpinOnce();
            }
        }

        private void SetStateLck(uint lck)
        {
            SpinWait spin = new SpinWait();
            while (true)
            {
                uint init = _state;
                uint val = init & 0b1100 | lck & 0b11;
                if (Interlocked.CompareExchange(ref _state, val, init) == init)
                {
                    break;
                }
                spin.SpinOnce();
            }
        }

        private void AddStateLck(uint lck)
        {
            SpinWait spin = new SpinWait();
            while (true)
            {
                uint init = _state;
                uint val = init & 0b1111 | lck & 0b11;
                if (Interlocked.CompareExchange(ref _state, val, init) == init)
                {
                    break;
                }
                spin.SpinOnce();
            }
        }

        private void UnsetStateLck(uint lck)
        {
            SpinWait spin = new SpinWait();
            while (true)
            {
                uint init = _state;
                uint val = (init & 0b1111) & ~(lck & 0b11);
                if (Interlocked.CompareExchange(ref _state, val, init) == init)
                {
                    break;
                }
                spin.SpinOnce();
            }
        }

        [Obsolete("This method is not yet implemented correctly", true)]
        private void SetStateModLck(uint mod, uint lck)
        {
            SpinWait spin = new SpinWait();
            while (true) { 
                uint init = _state;
                uint val = (mod << 2) & 0b1100 | lck & 0b11;
                if (Interlocked.CompareExchange(ref _state, val, init) == init)
                {
                    break;
                }
                spin.SpinOnce();
            }
        }

        private uint GetMod()
        {

            return (_state >> 2) & 0b11;
        }

        private uint GetLck()
        {
            return _state & 0b11;
        }

        private (uint,uint) GetState()
        {
            uint mod  = (_state >> 2) & 0b11;
            uint lck = _state & 0b11;

            return (mod, lck);
        }
    }
}
