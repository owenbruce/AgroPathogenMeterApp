﻿using Microsoft.AppCenter.Crashes;
using PalmSens;
using PalmSens.Comm;
using PalmSens.Devices;
using PalmSens.Plottables;
using PalmSens.Techniques;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgroPathogenMeterApp.Droid
{
    public class PSCommSimple
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PSCommSimple" /> class.
        /// This class handles is used to perform measurements and control the device manually.
        /// It requires a reference to the platform specific instance of the class,
        /// i.e. PSCommSimpleWinForms, PSCommSimpleWPF or PSCommSimpleXamarin
        /// </summary>
        /// <param name="platform">The reference to the platform specific PSCommSimple class.</param>
        /// <exception cref="System.ArgumentNullException">Platform cannot be null</exception>
        public PSCommSimple(IPlatform platform)
        {
            _platform = platform ?? throw new ArgumentNullException("Platform cannot be null");
        }

        #region Properties

        /// <summary>
        /// The platform specific interface for WinForms, WPF and Xamarin support
        /// </summary>
        private IPlatform _platform = null;

        /// <summary>
        /// The connected device's CommManager
        /// </summary>
        private CommManager _comm;

        /// <summary>
        /// The task completion source used to obtain the active measurement in the Measure and MeasureAsync functions
        /// </summary>
        private TaskCompletionSource<SimpleMeasurement> _taskCompletionSource = null;

        /// <summary>
        /// Gets or sets the CommManager and (un)subscribes the corresponding events.
        /// </summary>
        /// <value>
        /// The CommManager.
        /// </value>
        public CommManager Comm
        {
            get { return _comm; }
            set
            {
                if (_comm != null) //Unsubscribe events
                {
                    _comm.BeginMeasurement -= _comm_BeginMeasurement;
                    _comm.BeginMeasurementAsync -= _comm_BeginMeasurementAsync;
                    _comm.EndMeasurement -= _comm_EndMeasurement;
                    _comm.EndMeasurementAsync -= _comm_EndMeasurementAsync;
                    _comm.BeginReceiveCurve -= _comm_BeginReceiveCurve;
                    _comm.ReceiveStatus -= _comm_ReceiveStatus;
                    _comm.ReceiveStatusAsync -= _comm_ReceiveStatusAsync;
                    _comm.StateChanged -= _comm_StateChanged;
                    _comm.StateChangedAsync -= _comm_StateChangedAsync;
                    _comm.Disconnected -= _comm_Disconnected;
                    _comm.CommErrorOccurred -= _comm_CommErrorOccurred;
                }
                _comm = value;
                if (_comm != null) //Subscribe events
                {
                    _comm.BeginMeasurement += _comm_BeginMeasurement;
                    _comm.BeginMeasurementAsync += _comm_BeginMeasurementAsync;
                    _comm.EndMeasurement += _comm_EndMeasurement;
                    _comm.EndMeasurementAsync += _comm_EndMeasurementAsync;
                    _comm.BeginReceiveCurve += _comm_BeginReceiveCurve;
                    _comm.ReceiveStatus += _comm_ReceiveStatus;
                    _comm.ReceiveStatusAsync += _comm_ReceiveStatusAsync;
                    _comm.StateChanged += _comm_StateChanged;
                    _comm.StateChangedAsync += _comm_StateChangedAsync;
                    _comm.Disconnected += _comm_Disconnected;
                    _comm.CommErrorOccurred += _comm_CommErrorOccurred;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="PSCommSimple"/> is connected to a device.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool Connected { get { return Comm != null; } }

        /// <summary>
        /// Gets the connected device type.
        /// </summary>
        /// <value>
        /// The connected device type.
        /// </value>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        public enumDeviceType ConnectedDevice
        {
            get
            {
                if (_comm == null)
                    throw new NullReferenceException("Not connected to a device.");
                return _comm.DeviceType;
            }
        }

        /// <summary>
        /// Gets the state of the device.
        /// </summary>
        /// <value>
        /// The state of the device.
        /// </value>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        public CommManager.DeviceState DeviceState
        {
            get
            {
                if (_comm == null)
                    throw new NullReferenceException("Not connected to a device.");
                return _comm.State;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the connected device's [cell is on].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [cell is on]; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        public bool IsCellOn
        {
            get
            {
                if (_comm == null)
                    throw new NullReferenceException("Not connected to a device.");
                return _comm.CellOn;
            }
        }

        /// <summary>
        /// Gets the capabilities of the connected device.
        /// </summary>
        /// <value>
        /// The device capabilities.
        /// </value>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        public DeviceCapabilities Capabilities
        {
            get
            {
                if (_comm == null)
                    throw new NullReferenceException("Not connected to a device.");
                return _comm.Capabilities;
            }
        }

        /// <summary>
        /// Determines whether [the specified method] is compatible with the device.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>
        ///   <c>true</c> if the method is valid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidMethod(Method method)
        {
            ValidateMethod(method, out bool valid, out List<string> errors);
            return valid;
        }

        /// <summary>
        /// The active measurement
        /// </summary>
        private Measurement _activeMeasurement;

        /// <summary>
        /// Gets or sets the active measurement manages the subscription to its events,
        /// the active simple measurement and the active curves.
        /// </summary>
        /// <value>
        /// The active measurement.
        /// </value>
        private Measurement ActiveMeasurement
        {
            get { return _activeMeasurement; }
            set
            {
                _activeMeasurement = value;
                if (_activeMeasurement != null)
                    _activeSimpleMeasurement = new SimpleMeasurement(_activeMeasurement);
            }
        }

        /// <summary>
        /// The active SimpleMeasurement
        /// </summary>
        private SimpleMeasurement _activeSimpleMeasurement;

        #endregion Properties

        #region Functions

        /// <summary>
        /// Disconnects from the connected device.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        public void Disconnect()
        {
            try
            {
                _platform.Disconnect(_comm);
                Comm = null;
                _activeMeasurement = null;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                throw new NullReferenceException("Not connected to a device.");
            }
        }

        /// <summary>
        /// Disconnects from the connected device.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        public async Task DisconnectAsync()
        {
            try
            {
                await Task.Run(() =>
                { //The disconnect function should not be run using CommManager.ClientConnection.RunAsync()
                    _platform.Disconnect(_comm);
                    Comm = null;
                    _activeMeasurement = null;
                });
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                throw new NullReferenceException("Not connected to a device.");
            }
        }

        /// <summary>
        /// Runs a measurement as specified in the method on the connected device.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <param name="muxChannel">The mux channel to measure on.</param>
        /// <returns>
        /// A SimpleMeasurement instance containing all the data related to the measurement.
        /// </returns>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        /// <exception cref="System.ArgumentException">Method is incompatible with the connected device.</exception>
        /// <exception cref="System.Exception">Could not start measurement.</exception>
        public SimpleMeasurement Measure(Method method, int muxChannel)
        {
            _activeMeasurement = null;
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device.");

            //Update the autoranging depending on the current ranges supported by the connected device
            if (Connected)
                method.Ranging.SupportedCurrentRanges = Capabilities.SupportedRanges;

            //Check whether method is compatible with the connected device
            ValidateMethod(method, out bool isValidMethod, out List<string> errors);
            if (!isValidMethod)
                throw new ArgumentException("Method is incompatible with the connected device.");

            //Init task to wait for the active measurement to be initiated by CommManager.Measure()
            _taskCompletionSource = new TaskCompletionSource<SimpleMeasurement>();
            _comm.BeginMeasurement += GetActiveMeasurement;

            //Start the measurement on the connected device, this triggers an event that updates _activeMeasurement
            string error = Run(() => _comm.Measure(method, muxChannel));
            if (!(string.IsNullOrEmpty(error)))
                throw new Exception($"Could not start measurement: {error}");

            _taskCompletionSource.Task.Wait();

            return _taskCompletionSource.Task.Result;
        }

        /// <summary>
        /// Runs a measurement as specified in the method on the connected device.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <param name="muxChannel">The mux channel to measure on.</param>
        /// <returns>
        /// A SimpleMeasurement instance containing all the data related to the measurement.
        /// </returns>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        /// <exception cref="System.ArgumentException">Method is incompatible with the connected device.</exception>
        /// <exception cref="System.Exception">Could not start measurement.</exception>
        public async Task<SimpleMeasurement> MeasureAsync(Method method, int muxChannel, TaskBarrier taskBarrier = null)
        {
            _activeMeasurement = null;
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device.");

            //Update the autoranging depending on the current ranges supported by the connected device
            if (Connected)
                method.Ranging.SupportedCurrentRanges = Capabilities.SupportedRanges;

            //Check whether method is compatible with the connected device
            ValidateMethod(method, out bool isValidMethod, out List<string> errors);
            if (!isValidMethod)
                throw new ArgumentException("Method is incompatible with the connected device.");

            //Init task to wait for the active measurement to be initiated by CommManager.MeasureAsync()
            _taskCompletionSource = new TaskCompletionSource<SimpleMeasurement>();
            _comm.BeginMeasurementAsync += GetActiveMeasurementAsync;

            string error = "";

            //Start the measurement on the connected device, this triggers an event that updates _activeMeasurement
            error = await RunAsync<string>(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (_comm == null)
                    throw new NullReferenceException("Not connected to a device");
                return await _comm.MeasureAsync(method, muxChannel, taskBarrier);
            });

            if (!(string.IsNullOrEmpty(error)))
                throw new Exception($"Could not start measurement: {error}");

            return await _taskCompletionSource.Task;
        }

        /// <summary>
        /// Gets the active measurement when the BeginMeasurement event is raised.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="newMeasurement">The new measurement.</param>
        /// <exception cref="NotImplementedException"></exception>
        private void GetActiveMeasurement(object sender, ActiveMeasurement m)
        {
            _comm.BeginMeasurement -= GetActiveMeasurement;
            ActiveMeasurement = m;
            if (ActiveMeasurement.Method is ImpedimetricMethod eis)
                _activeSimpleMeasurement.NewSimpleCurve(PalmSens.Data.DataArrayType.ZRe, PalmSens.Data.DataArrayType.ZIm, "Nyquist", true); //Create a nyquist curve by default
            _taskCompletionSource.SetResult(_activeSimpleMeasurement);
        }

        /// <summary>
        /// Gets the active measurement asynchronous when the BeginMeasurementAsync event is raised.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private async Task GetActiveMeasurementAsync(object sender, ActiveMeasurement m)
        {
            _comm.BeginMeasurementAsync -= GetActiveMeasurementAsync;
            ActiveMeasurement = m;
            if (ActiveMeasurement is ImpedimetricMeasurement eis)
                _activeSimpleMeasurement.NewSimpleCurve(PalmSens.Data.DataArrayType.ZRe, PalmSens.Data.DataArrayType.ZIm, "Nyquist", true); //Create a nyquist curve by default
            _taskCompletionSource.SetResult(_activeSimpleMeasurement);
        }

        /// <summary>
        /// Runs a measurement as specified in the method on the connected device.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <returns>A SimpleMeasurement instance containing all the data related to the measurement.</returns>
        public SimpleMeasurement Measure(Method method)
        {
            if (method.MuxMethod == MuxMethod.Sequentially)
                return Measure(method, method.GetNextSelectedMuxChannel(-1));
            else
                return Measure(method, -1);
        }

        /// <summary>
        /// Runs a measurement as specified in the method on the connected device.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <returns>A SimpleMeasurement instance containing all the data related to the measurement.</returns>
        public async Task<SimpleMeasurement> MeasureAsync(Method method, TaskBarrier taskBarrier = null)
        {
            if (method.MuxMethod == MuxMethod.Sequentially)
                return await MeasureAsync(method, method.GetNextSelectedMuxChannel(-1), taskBarrier);
            else
                return await MeasureAsync(method, -1, taskBarrier);
        }

        /// <summary>
        /// Aborts the active measurement.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        /// <exception cref="System.Exception">Device is not measuring.</exception>
        public void AbortMeasurement()
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device.");
            if (_comm.ActiveMeasurement == null)
                throw new Exception("Device is not measuring.");

            Run(() => _comm.Abort());
        }

        /// <summary>
        /// Aborts the current active measurement.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        /// <exception cref="System.Exception">The device is not currently performing measurement</exception>
        public async Task AbortMeasurementAsync()
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device.");
            if (_comm.ActiveMeasurement == null)
                throw new Exception("Device is not measuring.");

            await RunAsync(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (_comm == null)
                    throw new NullReferenceException("Not connected to a device");
                if (_comm.ActiveMeasurement == null)
                    throw new Exception("Device is not measuring.");
                await _comm.AbortAsync();
            });
        }

        /// <summary>
        /// Turns the cell on.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a device</exception>
        /// <exception cref="System.Exception">Device must be in idle mode for manual control</exception>
        public void TurnCellOn()
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device");
            if (_comm.State != CommManager.DeviceState.Idle)
                throw new Exception("Device must be in idle mode for manual control");
            if (_comm.CellOn)
                return;

            Run(() => { _comm.CellOn = true; });
        }

        /// <summary>
        /// Turns the cell on.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a device</exception>
        /// <exception cref="System.Exception">Device must be in idle mode for manual control</exception>
        public async Task TurnCellOnAsync()
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device");
            if (_comm.State != CommManager.DeviceState.Idle)
                throw new Exception("Device must be in idle mode for manual control");
            if (_comm.CellOn)
                return;

            await RunAsync(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (_comm == null)
                    throw new NullReferenceException("Not connected to a device");
                if (_comm.State != CommManager.DeviceState.Idle)
                    throw new Exception("Device must be in idle mode for manual control");
                if (_comm.CellOn)
                    return;
                await _comm.SetCellOnAsync(true);
            });
        }

        /// <summary>
        /// Turns the cell off.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a device</exception>
        /// <exception cref="System.Exception">Device must be in idle mode for manual control</exception>
        public void TurnCellOff()
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device");
            if (_comm.State != CommManager.DeviceState.Idle)
                throw new Exception("Device must be in idle mode for manual control");
            if (!_comm.CellOn)
                return;

            Run(() => { _comm.CellOn = false; });
        }

        /// <summary>
        /// Turns the cell off.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a device</exception>
        /// <exception cref="System.Exception">Device must be in idle mode for manual control</exception>
        public async Task TurnCellOffAsync()
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device");
            if (await _comm.GetStateAsync() != CommManager.DeviceState.Idle)
                throw new Exception("Device must be in idle mode for manual control");
            if (!_comm.CellOn)
                return;

            await RunAsync(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (_comm == null)
                    throw new NullReferenceException("Not connected to a device");
                if (_comm.State != CommManager.DeviceState.Idle)
                    throw new Exception("Device must be in idle mode for manual control");
                if (!_comm.CellOn)
                    return;
                await _comm.SetCellOnAsync(false);
            });
        }

        /// <summary>
        /// Sets the cell potential.
        /// </summary>
        /// <param name="potential">The potential.</param>
        /// <exception cref="System.NullReferenceException">Not connected to a device</exception>
        /// <exception cref="System.Exception">Device must be in idle mode for manual control</exception>
        public void SetCellPotential(float potential)
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device");
            if (_comm.State != CommManager.DeviceState.Idle)
                throw new Exception("Device must be in idle mode for manual control");

            Run(() => { _comm.Potential = potential; });
        }

        /// <summary>
        /// Sets the cell potential.
        /// </summary>
        /// <param name="potential">The potential.</param>
        /// <exception cref="System.NullReferenceException">Not connected to a device</exception>
        /// <exception cref="System.Exception">Device must be in idle mode for manual control</exception>
        public async Task SetCellPotentialAsync(float potential)
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device");
            if (await _comm.GetStateAsync() != CommManager.DeviceState.Idle)
                throw new Exception("Device must be in idle mode for manual control");

            await RunAsync(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (_comm == null)
                    throw new NullReferenceException("Not connected to a device");
                if (await _comm.GetStateAsync() != CommManager.DeviceState.Idle)
                    throw new Exception("Device must be in idle mode for manual control");
                await _comm.SetPotentialAsync(potential);
            });
        }

        /// <summary>
        /// Reads the cell potential.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Not connected to a device</exception>
        /// <exception cref="Exception">Device must be in idle mode for manual control</exception>
        public float ReadCellPotential()
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device");
            if (_comm.State != CommManager.DeviceState.Idle)
                throw new Exception("Device must be in idle mode for manual control");

            return Run<float>(() => { return _comm.Potential; });
        }

        /// <summary>
        /// Reads the cell potential.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Not connected to a device</exception>
        /// <exception cref="Exception">Device must be in idle mode for manual control</exception>
        public async Task<float> ReadCellPotentialAsync()
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device");
            if (_comm.State != CommManager.DeviceState.Idle)
                throw new Exception("Device must be in idle mode for manual control");

            return await RunAsync<float>(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (_comm == null)
                    throw new NullReferenceException("Not connected to a device");
                if (_comm.State != CommManager.DeviceState.Idle)
                    throw new Exception("Device must be in idle mode for manual control");
                return await _comm.GetPotentialAsync();
            });
        }

        /// <summary>
        /// Sets the cell current.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <exception cref="System.NullReferenceException">Not connected to a device</exception>
        /// <exception cref="System.Exception">Device must be in idle mode for manual control</exception>
        public void SetCellCurrent(float current)
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device");
            if (_comm.State != CommManager.DeviceState.Idle)
                throw new Exception("Device must be in idle mode for manual control");
            if (!Capabilities.IsGalvanostat)
                throw new Exception("Device does not support Galvanostat mode");

            Run(() => { _comm.Current = current; });
        }

        /// <summary>
        /// Sets the cell current.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <exception cref="System.NullReferenceException">Not connected to a device</exception>
        /// <exception cref="System.Exception">Device must be in idle mode for manual control</exception>
        public async Task SetCellCurrentAsync(float current)
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device");
            if (await _comm.GetStateAsync() != CommManager.DeviceState.Idle)
                throw new Exception("Device must be in idle mode for manual control");
            if (!Capabilities.IsGalvanostat)
                throw new Exception("Device does not support Galvanostat mode");

            await RunAsync(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (_comm == null)
                    throw new NullReferenceException("Not connected to a device");
                if (await _comm.GetStateAsync() != CommManager.DeviceState.Idle)
                    throw new Exception("Device must be in idle mode for manual control");
                if (!Capabilities.IsGalvanostat)
                    throw new Exception("Device does not support Galvanostat mode");
                await _comm.SetCurrentAsync(current);
            });
        }

        /// <summary>
        /// Reads the cell current.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Not connected to a device</exception>
        /// <exception cref="Exception">Device must be in idle mode for manual control</exception>
        public float ReadCellCurrent()
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device");
            if (_comm.State != CommManager.DeviceState.Idle)
                throw new Exception("Device must be in idle mode for manual control");

            return Run<float>(() => { return _comm.Current; });
        }

        /// <summary>
        /// Reads the cell current.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Not connected to a device</exception>
        /// <exception cref="Exception">Device must be in idle mode for manual control</exception>
        public async Task<float> ReadCellCurrentAsync()
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device");
            if (_comm.State != CommManager.DeviceState.Idle)
                throw new Exception("Device must be in idle mode for manual control");

            return await RunAsync<float>(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (_comm == null)
                    throw new NullReferenceException("Not connected to a device");
                if (_comm.State != CommManager.DeviceState.Idle)
                    throw new Exception("Device must be in idle mode for manual control");
                return await _comm.GetCurrentAsync();
            });
        }

        /// <summary>
        /// Sets the current range.
        /// </summary>
        /// <param name="currentRange">The current range.</param>
        /// <exception cref="System.NullReferenceException">Not connected to a device</exception>
        /// <exception cref="System.Exception">Device must be in idle mode for manual control</exception>
        public void SetCurrentRange(CurrentRange currentRange)
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device");
            if (_comm.State != CommManager.DeviceState.Idle)
                throw new Exception("Device must be in idle mode for manual control");

            Run(() => { _comm.CurrentRange = currentRange; });
        }

        /// <summary>
        /// Sets the current range.
        /// </summary>
        /// <param name="currentRange">The current range.</param>
        /// <exception cref="System.NullReferenceException">Not connected to a device</exception>
        /// <exception cref="System.Exception">Device must be in idle mode for manual control</exception>
        public async Task SetCurrentRangeAsync(CurrentRange currentRange)
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device");
            if (await _comm.GetStateAsync() != CommManager.DeviceState.Idle)
                throw new Exception("Device must be in idle mode for manual control");

            await RunAsync(async () =>
            {
                //Need to check again as the task can be scheduled to run at a later point after which this could have changed
                if (_comm == null)
                    throw new NullReferenceException("Not connected to a device");
                if (await _comm.GetStateAsync() != CommManager.DeviceState.Idle)
                    throw new Exception("Device must be in idle mode for manual control");
                await _comm.SetCurrentRangeAsync(currentRange);
            });
        }

        /// <summary>
        /// Validates whether the specified method is compatible with the capabilities of the connected device.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <param name="isValidMethod">if set to <c>true</c> [is valid method].</param>
        /// <param name="errors">The errors.</param>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        /// <exception cref="System.ArgumentNullException">The specified method cannot be null.</exception>
        public void ValidateMethod(Method method, out bool isValidMethod, out List<string> errors)
        {
            if (_comm == null)
                throw new NullReferenceException("Not connected to a device.");
            if (method == null)
                throw new ArgumentNullException("The specified method cannot be null.");
            errors = new List<string>();

            //Get a list of method compatability warnings and errors for the connected device
            List<MethodError> methodErrors = method.Validate(_comm.Capabilities);

            //Check wheteher the device can perform the measurement described in the method
            isValidMethod = !(methodErrors.Where(c => c.IsFatal == true).Any());

            //Build a list of the warnings and errors
            foreach (MethodError error in methodErrors)
                errors.Add($"{error.Parameter.ToString()}: {error.Message}");
        }

        /// <summary>
        /// Adds the active curve and its respective to the collection and subscribes to its events.
        /// </summary>
        /// <param name="activeCurve">The active curve.</param>
        private void OnSimpleCurveStartReceivingData(Curve activeCurve)
        {
            if (activeCurve == null)
                return;

            SimpleCurve activeSimpleCurve = _activeSimpleMeasurement.SimpleCurveCollection.Where(sc => sc.Curve == activeCurve).FirstOrDefault();

            if (activeSimpleCurve == null)
            {
                activeSimpleCurve = new SimpleCurve(activeCurve, _activeSimpleMeasurement);
                _activeSimpleMeasurement.AddSimpleCurve(activeSimpleCurve);
            }

            SimpleCurveStartReceivingData?.Invoke(this, activeSimpleCurve);
        }

        /// <summary>
        /// Safely run an Action delegate on the clientconnection.
        /// </summary>
        /// <param name="action">The action.</param>
        private void Run(Action action)
        {
            if (TaskScheduler.Current == _comm.ClientConnection.TaskScheduler)
                throw new Exception("The device can only execute one command at a time. Dead lock detected");
            _comm.ClientConnection.Semaphore.Wait();
            try { action(); }
            finally { _comm.ClientConnection.Semaphore.Release(); }
        }

        /// <summary>
        /// Safely run a Function delegate on the clientconnection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        private T Run<T>(Func<T> func)
        {
            if (TaskScheduler.Current == _comm.ClientConnection.TaskScheduler)
                throw new Exception("The device can only execute one command at a time. Dead lock detected");
            _comm.ClientConnection.Semaphore.Wait();
            try { return func(); }
            finally { _comm.ClientConnection.Semaphore.Release(); }
        }

        /// <summary>
        /// Runs an async Func delegate asynchronously on the clientconnections taskscheduler.
        /// </summary>
        /// <param name="func">The action.</param>
        /// <returns></returns>
        private async Task RunAsync(Func<Task> func)
        {
            await new SynchronizationContextRemover();
            await _comm.ClientConnection.RunAsync(func);
        }

        /// <summary>
        /// Runs an async Func delegate asynchronously on the clientconnections taskscheduler.
        /// </summary>
        /// <param name="func">The action.</param>
        /// <returns></returns>
        private async Task<T> RunAsync<T>(Func<Task<T>> func)
        {
            await new SynchronizationContextRemover();
            return await _comm.ClientConnection.RunAsync(func);
        }

        #endregion Functions

        #region events

        /// <summary>
        /// Occurs when a device status package is received, these packages are not sent during a measurement.
        /// </summary>
        public event StatusEventHandler ReceiveStatus;

        /// <summary>
        /// Casts ReceiveStatus events coming from a different thread to the UI thread when necessary.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StatusEventArgs" /> instance containing the device status.</param>
        /// <exception cref="System.NullReferenceException">Platform not set.</exception>
        private void _comm_ReceiveStatus(object sender, StatusEventArgs e)
        {
            if (_platform == null)
                throw new NullReferenceException("Platform not set.");
            if (_platform.InvokeIfRequired(new StatusEventHandler(_comm_ReceiveStatus), sender, e)) //Recast event to UI thread when necessary
                return;
            ReceiveStatus?.Invoke(this, e);
        }

        /// <summary>
        /// Casts ReceiveStatus events coming from a different thread to the UI thread when necessary.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StatusEventArgs" /> instance containing the device status.</param>
        /// <exception cref="System.NullReferenceException">Platform not set.</exception>
        private async Task _comm_ReceiveStatusAsync(object sender, StatusEventArgs e)
        {
            _comm_ReceiveStatus(sender, e);
        }

        /// <summary>
        /// Occurs at the start of a new measurement.
        /// </summary>
        public event EventHandler MeasurementStarted;

        /// <summary>
        /// Sets the ActiveMeasurement at the start of a measurement and casts BeginMeasurement events coming from a different thread to the UI thread when necessary.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="newMeasurement">The new measurement.</param>
        /// <exception cref="System.NullReferenceException">Platform not set.</exception>
        private void _comm_BeginMeasurement(object sender, ActiveMeasurement newMeasurement)
        {
            if (_platform == null)
                throw new NullReferenceException("Platform not set.");
            if (_platform.InvokeIfRequired(new CommManager.BeginMeasurementEventHandler(_comm_BeginMeasurement), sender, newMeasurement)) //Recast event to UI thread when necessary
                return;
            MeasurementStarted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Sets the ActiveMeasurement at the start of a measurement and casts BeginMeasurement events coming from a different thread to the UI thread when necessary.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The new measurement.</param>
        /// <exception cref="System.NullReferenceException">Platform not set.</exception>
        private async Task _comm_BeginMeasurementAsync(object sender, ActiveMeasurement e)
        {
            _comm_BeginMeasurement(sender, e);
        }

        /// <summary>
        /// Occurs when a measurement has ended.
        /// </summary>
        public event EventHandler MeasurementEnded;

        /// <summary>
        /// Sets the ActiveMeasurement to null at the end of the measurement and casts EndMeasurement events coming from a different thread to the UI thread when necessary.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NullReferenceException">Platform not set.</exception>
        private void _comm_EndMeasurement(object sender, EventArgs e)
        {
            if (_platform == null)
                throw new NullReferenceException("Platform not set.");
            if (_platform.InvokeIfRequired(new EventHandler(_comm_EndMeasurement), sender, e)) //Recast event to UI thread when necessary
                return;
            ActiveMeasurement = null;
            MeasurementEnded?.Invoke(this, e);
        }

        /// <summary>
        /// Sets the ActiveMeasurement to null at the end of the measurement and casts EndMeasurement events coming from a different thread to the UI thread when necessary.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NullReferenceException">Platform not set.</exception>
        private async Task _comm_EndMeasurementAsync(object sender, EventArgs e)
        {
            _comm_EndMeasurement(sender, e);
        }

        /// <summary>
        /// Adds the active Curve to the active SimpleMeasurement and casts BeginReceiveCurve events coming from a different thread to the UI thread when necessary.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CurveEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NullReferenceException">Platform not set.</exception>
        private void _comm_BeginReceiveCurve(object sender, CurveEventArgs e)
        {
            if (_platform == null)
                throw new NullReferenceException("Platform not set.");
            if (_platform.InvokeIfRequired(new CurveEventHandler(_comm_BeginReceiveCurve), sender, e)) //Recast event to UI thread when necessary
                return;
            OnSimpleCurveStartReceivingData(e.GetCurve());
        }

        /// <summary>
        /// EventHandler delegate with a reference to a SimpleCurve
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="activeSimpleCurve">The active simple curve.</param>
        public delegate void SimpleCurveStartReceivingDataHandler(Object sender, SimpleCurve activeSimpleCurve);

        /// <summary>
        /// Occurs when a new [SimpleCurve starts receiving data].
        /// </summary>
        public event SimpleCurveStartReceivingDataHandler SimpleCurveStartReceivingData;

        /// <summary>
        /// Occurs when the devive's [state changed].
        /// </summary>
        public event CommManager.StatusChangedEventHandler StateChanged;

        /// <summary>
        /// Casts StateChanged events coming from a different thread to the UI thread when necessary.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="CurrentState">State of the current.</param>
        /// <exception cref="System.NullReferenceException">Platform not set.</exception>
        private void _comm_StateChanged(object sender, CommManager.DeviceState CurrentState)
        {
            if (_platform == null)
                throw new NullReferenceException("Platform not set.");
            if (_platform.InvokeIfRequired(new CommManager.StatusChangedEventHandler(_comm_StateChanged), sender, CurrentState)) //Recast event to UI thread when necessary
                return;
            StateChanged?.Invoke(this, CurrentState);
        }

        /// <summary>
        /// Casts StateChanged events coming from a different thread to the UI thread when necessary.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">State of the current.</param>
        /// <exception cref="System.NullReferenceException">Platform not set.</exception>
        private async Task _comm_StateChangedAsync(object sender, CommManager.DeviceState e)
        {
            _comm_StateChanged(sender, e);
        }

        /// <summary>
        /// Occurs when a device is [disconnected].
        /// </summary>
        public event DisconnectedEventHandler Disconnected;

        /// <summary>
        /// Raises the Disconnected event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void _comm_Disconnected(object sender, EventArgs e)
        {
            if (_platform == null)
                throw new NullReferenceException("Platform not set.");
            if (_platform.InvokeIfRequired(new EventHandler(_comm_Disconnected), sender, e)) //Recast event to UI thread when necessary
                return;
            Disconnected?.Invoke(this, _commErrorException);
            _commErrorException = null;
        }

        /// <summary>
        /// The latest comm error exception, this is used for the disconnected event and is set back to null directly after it is raised
        /// </summary>
        private Exception _commErrorException = null;

        /// <summary>
        /// Comms the comm error occorred.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void _comm_CommErrorOccurred(object sender, Exception exception)
        {
            if (_platform == null)
                throw new NullReferenceException("Platform not set.");
            if (_platform.InvokeIfRequired(new CommManager.EventHandlerCommErrorOccurred(_comm_CommErrorOccurred), sender, exception)) //Recast event to UI thread when necessary
                return;
            _commErrorException = exception;
        }

        #endregion events

        public void Dispose()
        {
            if (Connected)
                _comm.Dispose();
            _comm = null;
            ActiveMeasurement = null;
            Disconnected = null;
            MeasurementEnded = null;
            MeasurementStarted = null;
            ReceiveStatus = null;
            StateChanged = null;
            SimpleCurveStartReceivingData = null;
        }
    }

    /// <summary>
    /// Delegate for the Disconnected event
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="CommErrorException">The comm error exception, this is only available when device was disconnected due to a communication error.</param>
    public delegate void DisconnectedEventHandler(Object sender, Exception CommErrorException);
}