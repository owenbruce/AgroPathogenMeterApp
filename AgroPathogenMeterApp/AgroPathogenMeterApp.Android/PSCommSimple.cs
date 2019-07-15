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
    public partial class PSCommSimple
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
            if (platform == null)
            {
                throw new ArgumentNullException("Platform cannot be null");
            }

            _platform = platform;
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
                    _comm.EndMeasurement -= _comm_EndMeasurement;
                    _comm.BeginReceiveCurve -= _comm_BeginReceiveCurve;
                    _comm.BeginReceiveEISData -= _comm_BeginReceiveEISData;
                    _comm.ReceiveStatus -= _comm_ReceiveStatus;
                    _comm.StateChanged -= _comm_StateChanged;
                }
                _comm = value;
                if (_comm != null) //Subscribe events
                {
                    _comm.BeginMeasurement += _comm_BeginMeasurement;
                    _comm.EndMeasurement += _comm_EndMeasurement;
                    _comm.BeginReceiveCurve += _comm_BeginReceiveCurve;
                    _comm.BeginReceiveEISData += _comm_BeginReceiveEISData;
                    _comm.ReceiveStatus += _comm_ReceiveStatus;
                    _comm.StateChanged += _comm_StateChanged;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="PSCommSimple"/> is connected to a device.
        /// </summary>
        /// <value>
        ///   <c>true</c> if connected; otherwise, <c>false</c>.
        /// </value>
        public bool Connected
        {
            get { return Comm != null; }
        }

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
                {
                    throw new NullReferenceException("Not connected to a device.");
                }

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
                {
                    throw new NullReferenceException("Not connected to a device.");
                }

                return _comm.State;
            }
        }

        /// <summary>
        /// Gets the state of the device.
        /// </summary>
        /// <value>
        /// The state of the device.
        /// </value>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        public async Task<CommManager.DeviceState> GetDeviceStateAsync()
        {
            if (_comm == null)
            {
                throw new NullReferenceException("Not connected to a device.");
            }

            return await _comm.GetStateAsync();
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
                {
                    throw new NullReferenceException("Not connected to a device.");
                }

                return _comm.CellOn;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the connected device's [cell is on].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [cell is on]; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        public async Task<bool> IsCellOnAsync()
        {
            if (_comm == null)
            {
                throw new NullReferenceException("Not connected to a device.");
            }

            return await _comm.GetCellOnAsync();
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
                {
                    throw new NullReferenceException("Not connected to a device.");
                }

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
            bool valid;
            List<string> errors;
            ValidateMethod(method, out valid, out errors);
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
                {
                    _activeSimpleMeasurement = new SimpleMeasurement(_activeMeasurement);
                }
                else
                {
                    _activeSimpleMeasurement = null;
                    ClearActiveCurves();
                }
            }
        }

        /// <summary>
        /// The active SimpleMeasurement
        /// </summary>
        private SimpleMeasurement _activeSimpleMeasurement;

        /// <summary>
        /// Collection of active curves and their respective simplecurves
        /// </summary>
        private Dictionary<Curve, SimpleCurve> _activeCurves = new Dictionary<Curve, SimpleCurve>();

        #endregion Properties

        #region Functions

        /// <summary>
        /// Disconnects from the connected device.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        public void Disconnect()
        {
            try { _platform.Disconnect(_comm); }
            catch (Exception ex)
            {
                throw new NullReferenceException("Not connected to a device.");
            }
            Comm = null;
            _activeMeasurement = null;
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
            {
                throw new NullReferenceException("Not connected to a device.");
            }

            //Check whether method is compatible with the connected device
            bool isValidMethod;
            List<string> errors;
            ValidateMethod(method, out isValidMethod, out errors);
            if (!isValidMethod)
            {
                throw new ArgumentException("Method is incompatible with the connected device.");
            }

            //Start the measurement on the connected device, this triggers an event that updates _activeMeasurement
            string error = _comm.Measure(method, muxChannel);
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception($"Could not start measurement: {error}");
            }

            return _activeSimpleMeasurement;
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
        public async Task<SimpleMeasurement> MeasureAsync(Method method, int muxChannel)
        {
            try
            {
                await _comm.ClientConnection.Semaphore.WaitAsync();
                _activeMeasurement = null;
                if (_comm == null)
                {
                    throw new NullReferenceException("Not connected to a device.");
                }

                //Check whether method is compatible with the connected device
                bool isValidMethod;
                List<string> errors;
                ValidateMethod(method, out isValidMethod, out errors);
                if (!isValidMethod)
                {
                    throw new ArgumentException("Method is incompatible with the connected device.");
                }

                //Start the measurement on the connected device, this triggers an event that updates _activeMeasurement
                string error = await _comm.MeasureAsync(method, muxChannel);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception($"Could not start measurement: {error}");
                }

                return _activeSimpleMeasurement;
            }
            finally
            {
                _comm.ClientConnection.Semaphore.Release();
            }
        }

        /// <summary>
        /// Runs a measurement as specified in the method on the connected device.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <returns>A SimpleMeasurement instance containing all the data related to the measurement.</returns>
        public SimpleMeasurement Measure(Method method)
        {
            return method.MuxMethod == MuxMethod.Sequentially ? Measure(method, method.GetNextSelectedMuxChannel(-1)) : Measure(method, -1);
        }

        /// <summary>
        /// Runs a measurement as specified in the method on the connected device.
        /// </summary>
        /// <param name="method">The method containing the measurement parameters.</param>
        /// <returns>A SimpleMeasurement instance containing all the data related to the measurement.</returns>
        public async Task<SimpleMeasurement> MeasureAsync(Method method)
        {
            return method.MuxMethod == MuxMethod.Sequentially
                ? await MeasureAsync(method, method.GetNextSelectedMuxChannel(-1))
                : await MeasureAsync(method, -1);
        }

        /// <summary>
        /// Aborts the active measurement.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        /// <exception cref="System.Exception">Device is not measuring.</exception>
        public void AbortMeasurement()
        {
            if (_comm == null)
            {
                throw new NullReferenceException("Not connected to a device.");
            }

            if (_comm.ActiveMeasurement == null)
            {
                throw new Exception("Device is not measuring.");
            }

            _comm.Abort();
        }

        /// <summary>
        /// Aborts the current active measurement.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a device.</exception>
        /// <exception cref="System.Exception">The device is not currently performing measurement</exception>
        public async Task AbortMeasurementAsync()
        {
            if (_comm == null)
            {
                throw new NullReferenceException("Not connected to a device.");
            }

            if (_comm.ActiveMeasurement == null)
            {
                throw new Exception("Device is not measuring.");
            }

            await _comm.AbortAsync();
        }

        /// <summary>
        /// Turns the cell on.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a device</exception>
        /// <exception cref="System.Exception">Device must be in idle mode for manual control</exception>
        public void TurnCellOn()
        {
            if (_comm == null)
            {
                throw new NullReferenceException("Not connected to a device");
            }

            if (_comm.State != CommManager.DeviceState.Idle)
            {
                throw new Exception("Device must be in idle mode for manual control");
            }

            if (_comm.CellOn)
            {
                return;
            }

            _comm.CellOn = true;
        }

        /// <summary>
        /// Turns the cell on.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a device</exception>
        /// <exception cref="System.Exception">Device must be in idle mode for manual control</exception>
        public async Task TurnCellOnAsync()
        {
            if (_comm == null)
            {
                throw new NullReferenceException("Not connected to a device");
            }

            if (await _comm.GetStateAsync() != CommManager.DeviceState.Idle)
            {
                throw new Exception("Device must be in idle mode for manual control");
            }

            if (await _comm.GetCellOnAsync())
            {
                return;
            }

            await _comm.SetCellOnAsync(true);
        }

        /// <summary>
        /// Turns the cell off.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a device</exception>
        /// <exception cref="System.Exception">Device must be in idle mode for manual control</exception>
        public void TurnCellOff()
        {
            if (_comm == null)
            {
                throw new NullReferenceException("Not connected to a device");
            }

            if (_comm.State != CommManager.DeviceState.Idle)
            {
                throw new Exception("Device must be in idle mode for manual control");
            }

            if (!_comm.CellOn)
            {
                return;
            }

            _comm.CellOn = false;
        }

        /// <summary>
        /// Turns the cell off.
        /// </summary>
        /// <exception cref="System.NullReferenceException">Not connected to a device</exception>
        /// <exception cref="System.Exception">Device must be in idle mode for manual control</exception>
        public async Task TurnCellOffAsync()
        {
            if (_comm == null)
            {
                throw new NullReferenceException("Not connected to a device");
            }

            if (await _comm.GetStateAsync() != CommManager.DeviceState.Idle)
            {
                throw new Exception("Device must be in idle mode for manual control");
            }

            if (!await _comm.GetCellOnAsync())
            {
                return;
            }

            await _comm.SetCellOnAsync(false);
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
            {
                throw new NullReferenceException("Not connected to a device");
            }

            if (_comm.State != CommManager.DeviceState.Idle)
            {
                throw new Exception("Device must be in idle mode for manual control");
            }

            _comm.Potential = potential;
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
            {
                throw new NullReferenceException("Not connected to a device");
            }

            if (await _comm.GetStateAsync() != CommManager.DeviceState.Idle)
            {
                throw new Exception("Device must be in idle mode for manual control");
            }

            await _comm.SetPotentialAsync(potential);
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
            {
                throw new NullReferenceException("Not connected to a device");
            }

            if (_comm.State != CommManager.DeviceState.Idle)
            {
                throw new Exception("Device must be in idle mode for manual control");
            }

            if (!Capabilities.IsGalvanostat)
            {
                throw new Exception("Device does not support Galvanostat mode");
            }

            _comm.Current = current;
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
            {
                throw new NullReferenceException("Not connected to a device");
            }

            if (await _comm.GetStateAsync() != CommManager.DeviceState.Idle)
            {
                throw new Exception("Device must be in idle mode for manual control");
            }

            if (!Capabilities.IsGalvanostat)
            {
                throw new Exception("Device does not support Galvanostat mode");
            }

            await _comm.SetCurrentAsync(current);
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
            {
                throw new NullReferenceException("Not connected to a device");
            }

            if (_comm.State != CommManager.DeviceState.Idle)
            {
                throw new Exception("Device must be in idle mode for manual control");
            }

            _comm.CurrentRange = currentRange;
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
            {
                throw new NullReferenceException("Not connected to a device");
            }

            if (await _comm.GetStateAsync() != CommManager.DeviceState.Idle)
            {
                throw new Exception("Device must be in idle mode for manual control");
            }

            await _comm.SetCurrentRangeAsync(currentRange);
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
            {
                throw new NullReferenceException("Not connected to a device.");
            }

            if (method == null)
            {
                throw new ArgumentNullException("The specified method cannot be null.");
            }

            errors = new List<string>();

            //Get a list of method compatability warnings and errors for the connected device
            List<MethodError> methodErrors = method.Validate(_comm.Capabilities);

            //Check wheteher the device can perform the measurement described in the method
            isValidMethod = !(methodErrors.Where(c => c.IsFatal == true).Any());

            //Build a list of the warnings and errors
            foreach (MethodError error in methodErrors)
            {
                errors.Add($"{error.Parameter.ToString()}: {error.Message}");
            }
        }

        /// <summary>
        /// Adds the active curve and its respective to the collection and subscribes to its events.
        /// </summary>
        /// <param name="activeCurve">The active curve.</param>
        private void AddActiveCurve(Curve activeCurve)
        {
            if (activeCurve == null)
            {
                return;
            }

            if (_activeCurves.ContainsKey(activeCurve))
            {
                return;
            }

            activeCurve.NewDataAdded += ActiveCurve_NewDataAdded;
            activeCurve.Finished += ActiveCurve_Finished;

            SimpleCurve activeSimpleCurve = _activeSimpleMeasurement.SimpleCurveCollection.Where(sc => sc.Curve == activeCurve).FirstOrDefault();

            if (activeSimpleCurve == null)
            {
                activeSimpleCurve = new SimpleCurve(activeCurve, _activeSimpleMeasurement);
                _activeSimpleMeasurement.AddSimpleCurve(activeSimpleCurve);
            }

            _activeCurves.Add(activeCurve, activeSimpleCurve);
            SimpleCurveStartReceivingData?.Invoke(this, activeSimpleCurve);
        }

        /// <summary>
        /// Removes the active curve and its respective active simplecurve from the collection and unsubsscribes its events.
        /// </summary>
        /// <param name="activeCurve">The active curve.</param>
        private void RemoveActiveCurve(Curve activeCurve)
        {
            if (activeCurve == null)
            {
                return;
            }

            activeCurve.NewDataAdded -= ActiveCurve_NewDataAdded;
            activeCurve.Finished -= ActiveCurve_Finished;

            if (!_activeCurves.ContainsKey(activeCurve))
            {
                return;
            }

            _activeCurves.Remove(activeCurve);
        }

        /// <summary>
        /// Clears the all active curves and respective simplecurves.
        /// </summary>
        private void ClearActiveCurves()
        {
            List<Curve> activeCurves = _activeCurves.Keys.ToList();
            foreach (Curve activeCurve in activeCurves)
            {
                _activeCurves.Remove(activeCurve);
            }
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
            {
                throw new NullReferenceException("Platform not set.");
            }

            if (_platform.InvokeIfRequired(new StatusEventHandler(_comm_ReceiveStatus), sender, e)) //Recast event to UI thread when necessary
            {
                return;
            }

            ReceiveStatus?.Invoke(this, e);
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
            {
                throw new NullReferenceException("Platform not set.");
            }

            if (_platform.InvokeIfRequired(new CommManager.BeginMeasurementEventHandler(_comm_BeginMeasurement), sender, newMeasurement)) //Recast event to UI thread when necessary
            {
                return;
            }

            ActiveMeasurement = newMeasurement;
            MeasurementStarted?.Invoke(this, EventArgs.Empty);
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
            {
                throw new NullReferenceException("Platform not set.");
            }

            if (_platform.InvokeIfRequired(new EventHandler(_comm_EndMeasurement), sender, e)) //Recast event to UI thread when necessary
            {
                return;
            }

            ActiveMeasurement = null;
            MeasurementEnded?.Invoke(this, e);
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
            {
                throw new NullReferenceException("Platform not set.");
            }

            if (_platform.InvokeIfRequired(new CurveEventHandler(_comm_BeginReceiveCurve), sender, e)) //Recast event to UI thread when necessary
            {
                return;
            }

            AddActiveCurve(e.GetCurve());
        }

        /// <summary>
        /// Adds the active EISData to the active SimpleMeasurement and casts BeginReceiveEISData events coming from a different thread to the UI thread when necessary.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eisdata">The eisdata.</param>
        /// <exception cref="System.NullReferenceException">Platform not set.</exception>
        private void _comm_BeginReceiveEISData(object sender, EISData eisdata)
        {
            if (_platform == null)
            {
                throw new NullReferenceException("Platform not set.");
            }

            if (_platform.InvokeIfRequired(new EISDataEventHandler(_comm_BeginReceiveEISData), sender, eisdata)) //Recast event to UI thread when necessary
            {
                return;
            }
            //AddActiveCurve(eisdata); //FIXME add support for impedance
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
            {
                throw new NullReferenceException("Platform not set.");
            }

            if (_platform.InvokeIfRequired(new CommManager.StatusChangedEventHandler(_comm_StateChanged), sender, CurrentState)) //Recast event to UI thread when necessary
            {
                return;
            }

            StateChanged?.Invoke(this, CurrentState);
        }

        /// <summary>
        /// Raises the active curves new data added event when new data is added to the curve.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PalmSens.Data.ArrayDataAddedEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NullReferenceException">Platform not set.</exception>
        private void ActiveCurve_NewDataAdded(object sender, PalmSens.Data.ArrayDataAddedEventArgs e)
        {
            if (_platform == null)
            {
                throw new NullReferenceException("Platform not set.");
            }

            if (_platform.InvokeIfRequired(new Curve.NewDataAddedEventHandler(ActiveCurve_NewDataAdded), sender, e)) //Recast event to UI thread when necessary
            {
                return;
            }

            SimpleCurve activeSimpleCurve;
            if (_activeCurves.TryGetValue(sender as Curve, out activeSimpleCurve))
            {
                activeSimpleCurve.OnNewDataAdded(e);
            }
        }

        /// <summary>
        /// Removes an curve from the active curve collection when it is finished.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ActiveCurve_Finished(object sender, EventArgs e)
        {
            RemoveActiveCurve(sender as Curve);
        }

        #endregion events

        public void Dispose()
        {
            if (Connected)
                _comm.Dispose();
            ActiveMeasurement = null;
        }
    }
}