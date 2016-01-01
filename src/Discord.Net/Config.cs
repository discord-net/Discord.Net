using System;

namespace Discord
{
    public abstract class Config<T>
        where T : Config<T>
    {
        protected bool _isLocked;
        protected internal void Lock() { _isLocked = true; }
        protected void SetValue<U>(ref U storage, U value)
        {
            if (_isLocked)
                throw new InvalidOperationException("Unable to modify a discord client's configuration after it has been created.");
            storage = value;
        }

        public T Clone()
        {
            var config = MemberwiseClone() as T;
            config._isLocked = false;
            return config;
        }
    }
}
