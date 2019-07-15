using PalmSens.Comm;
using System;

namespace AgroPathogenMeterApp.Droid
{
    public interface IPlatform
    {
        /// <summary>
        /// Invokes if required.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        bool InvokeIfRequired(Delegate method, params object[] args);

        void Disconnect(CommManager comm);
    }
}