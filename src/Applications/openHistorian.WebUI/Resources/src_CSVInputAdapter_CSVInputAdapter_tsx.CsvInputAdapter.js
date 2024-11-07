"use strict";
(self["webpackChunkcsvinputadapter"] = self["webpackChunkcsvinputadapter"] || []).push([["src_CSVInputAdapter_CSVInputAdapter_tsx"],{

/***/ "./src/CSVInputAdapter/CSVInputAdapter.tsx":
/*!*************************************************!*\
  !*** ./src/CSVInputAdapter/CSVInputAdapter.tsx ***!
  \*************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var _Settings_CSVSettings__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./Settings/CSVSettings */ "./src/CSVInputAdapter/Settings/CSVSettings.tsx");
/* harmony import */ var _CSVOutput__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./CSVOutput */ "./src/CSVInputAdapter/CSVOutput.tsx");


const CSVInputAdapter = {
    Settings: _Settings_CSVSettings__WEBPACK_IMPORTED_MODULE_0__["default"],
    Output: _CSVOutput__WEBPACK_IMPORTED_MODULE_1__["default"]
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (CSVInputAdapter);


/***/ }),

/***/ "./src/CSVInputAdapter/CSVOutput.tsx":
/*!*******************************************!*\
  !*** ./src/CSVInputAdapter/CSVOutput.tsx ***!
  \*******************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _gpa_gemstone_common_pages__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @gpa-gemstone/common-pages */ "webpack/sharing/consume/default/@gpa-gemstone/common-pages/@gpa-gemstone/common-pages");
/* harmony import */ var _gpa_gemstone_common_pages__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_common_pages__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var moment__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! moment */ "webpack/sharing/consume/default/moment/moment");
/* harmony import */ var moment__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(moment__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @gpa-gemstone/gpa-symbols */ "webpack/sharing/consume/default/@gpa-gemstone/gpa-symbols/@gpa-gemstone/gpa-symbols");
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_3___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_3__);
/* harmony import */ var _gpa_gemstone_react_graph__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @gpa-gemstone/react-graph */ "./node_modules/@gpa-gemstone/react-graph/lib/index.js");
/* harmony import */ var _gpa_gemstone_helper_functions__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! @gpa-gemstone/helper-functions */ "./node_modules/@gpa-gemstone/helper-functions/lib/index.js");
/* harmony import */ var lodash__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! lodash */ "webpack/sharing/consume/default/lodash/lodash");
/* harmony import */ var lodash__WEBPACK_IMPORTED_MODULE_6___default = /*#__PURE__*/__webpack_require__.n(lodash__WEBPACK_IMPORTED_MODULE_6__);





//import { IStartDuration } from '../../../Common/TSX/global'; //needs a type eventually
//import { GrafanaQueries } from 'TSX/QueryFunctions';


const CSVOutputAdapter = (props) => {
    const containerRef = react__WEBPACK_IMPORTED_MODULE_0__.useRef(null);
    const { width } = (0,_gpa_gemstone_helper_functions__WEBPACK_IMPORTED_MODULE_5__.useGetContainerPosition)(containerRef);
    const [outputMeasurements, setOutputMeasurements] = react__WEBPACK_IMPORTED_MODULE_0__.useState([]);
    const [timeFilter, setTimeFilter] = react__WEBPACK_IMPORTED_MODULE_0__.useState({ start: moment__WEBPACK_IMPORTED_MODULE_2___default()().subtract(props.Settings.DefaultDuration, 'seconds').format('MM/DD/YYYY HH:mm:ss.SSS'), duration: props.Settings.DefaultDuration, unit: 's' });
    const [lines, setLines] = react__WEBPACK_IMPORTED_MODULE_0__.useState([]);
    const [status, setStatus] = react__WEBPACK_IMPORTED_MODULE_0__.useState('unintiated');
    react__WEBPACK_IMPORTED_MODULE_0__.useEffect(() => {
        const measurements = MeasurementStringToList(props.ConnectionParameters);
        if (!lodash__WEBPACK_IMPORTED_MODULE_6___default().isEqual(outputMeasurements, measurements))
            setOutputMeasurements(measurements);
    }, [props.ConnectionParameters]);
    react__WEBPACK_IMPORTED_MODULE_0__.useEffect(() => {
        const handles = outputMeasurements.map((outputMeasurement) => props.GrafanaQueryFuncs.Query({
            dataTypeIndex: 0,
            range: {
                from: timeFilter.start,
                to: moment__WEBPACK_IMPORTED_MODULE_2___default()(timeFilter.start).add(timeFilter.duration, timeFilter.unit).toISOString()
            },
            interval: moment__WEBPACK_IMPORTED_MODULE_2___default()(timeFilter.start).add(timeFilter.duration, timeFilter.unit).diff(moment__WEBPACK_IMPORTED_MODULE_2___default()(timeFilter.start)).toString(),
            maxDataPoints: 1000,
            targets: [{
                    refID: 'A',
                    target: outputMeasurement,
                    metadataSelections: []
                }],
            adhocFilters: [],
            excludedFlags: 0,
            excludeNormalFlags: false
        }));
        Promise.all(handles).then((responses) => {
            setLines(responses.map((d, index) => {
                const measurement = outputMeasurements[index];
                return {
                    Label: measurement,
                    Data: d
                };
            }));
            setStatus('idle');
        }).catch(() => setStatus('error'));
    }, [outputMeasurements, timeFilter]);
    return (react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'container-fluid d-flex flex-column h-100 p-0' },
        react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row m-0 pb-3' },
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12 p-0' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_common_pages__WEBPACK_IMPORTED_MODULE_1__.TimeFilter, { filter: timeFilter, setFilter: (start, end, unit, duration) => setTimeFilter({ start: start, duration: duration, unit: unit }), showQuickSelect: true, isHorizontal: true, dateTimeSetting: 'startWindow', timeZone: 'UTC', format: 'datetime-local', showHelpMessage: false }))),
        react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row m-0' },
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12 p-0', ref: containerRef }, status === 'loading' ? react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_3__.ReactIcons.SpiningIcon, null) :
                react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_graph__WEBPACK_IMPORTED_MODULE_4__.Plot, { defaultTdomain: [moment__WEBPACK_IMPORTED_MODULE_2___default()(timeFilter.start).valueOf(), moment__WEBPACK_IMPORTED_MODULE_2___default()(timeFilter.start).add(timeFilter.duration, timeFilter.unit).valueOf()], height: 200, width: width, Ylabel: 'Measurements', legend: 'right', yDomain: 'AutoValue', Tlabel: 'Duration', useMetricFactors: false }, lines.map((line, i) => { var _a, _b; return (react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_graph__WEBPACK_IMPORTED_MODULE_4__.Line, { key: i, data: (_b = (_a = line.Data) === null || _a === void 0 ? void 0 : _a[0]) === null || _b === void 0 ? void 0 : _b.datapoints.map(d => [d[1], d[0]]), color: 'blue', lineStyle: '-', legend: line.Label })); }))))));
};
const MeasurementStringToList = (params) => {
    const outputMeasurements = [];
    const columnsParam = params.find(param => param.Name === 'OutputMeasurements');
    if (columnsParam != null && columnsParam.Value != null) {
        const entries = columnsParam.Value.split(';');
        entries.forEach(entry => {
            const outputMeasurement = entry.trim();
            if (outputMeasurement != null && outputMeasurement !== '')
                outputMeasurements.push(outputMeasurement);
        });
    }
    return outputMeasurements;
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (CSVOutputAdapter);


/***/ }),

/***/ "./src/CSVInputAdapter/Settings/CSVSettings.tsx":
/*!******************************************************!*\
  !*** ./src/CSVInputAdapter/Settings/CSVSettings.tsx ***!
  \******************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @gpa-gemstone/react-forms */ "webpack/sharing/consume/default/@gpa-gemstone/react-forms/@gpa-gemstone/react-forms");
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var lodash__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! lodash */ "webpack/sharing/consume/default/lodash/lodash");
/* harmony import */ var lodash__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(lodash__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var _MeasurementSelector__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./MeasurementSelector */ "./src/CSVInputAdapter/Settings/MeasurementSelector.tsx");
/* harmony import */ var _ColumnSelector__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./ColumnSelector */ "./src/CSVInputAdapter/Settings/ColumnSelector.tsx");





const CSVInputAdapterSettingsUI = (props) => {
    var _a, _b, _c, _d, _e, _f, _g, _h, _j;
    const [settings, setSettings] = react__WEBPACK_IMPORTED_MODULE_0__.useState(null);
    react__WEBPACK_IMPORTED_MODULE_0__.useEffect(() => {
        setSettings(convertParametersToSettings(props.ConnectionParameters));
    }, [props.ConnectionParameters]);
    react__WEBPACK_IMPORTED_MODULE_0__.useEffect(() => {
        if (settings == null)
            return;
        const newParams = convertSettingsToParameters(settings, props.ConnectionParameters);
        const sortedNewParams = sortConParams(newParams);
        const sortedCurrentParams = sortConParams(props.ConnectionParameters);
        if (!lodash__WEBPACK_IMPORTED_MODULE_2___default().isEqual(sortedNewParams, sortedCurrentParams))
            props.SetConnectionParameters(newParams);
    }, [settings, props.SetConnectionParameters, props.ConnectionParameters]);
    react__WEBPACK_IMPORTED_MODULE_0__.useEffect(() => {
        const validationErrors = [];
        if (settings == null)
            return;
        if (settings.FramesPerSecond == null || settings.FramesPerSecond < 0 || settings.FramesPerSecond > 120)
            validationErrors.push('Adapter Frames Per Second must be greater than 0, less than 120 and non-empty.');
        if (settings.FileName == null || settings.FileName.length < 3)
            validationErrors.push('Adapter File Name must be greater than 3 characters and non-empty.');
        if (settings.InputInterval == null)
            validationErrors.push('Adapter Input Interval must be non-empty.');
        if (settings.SkipRows == null || settings.SkipRows < 0)
            validationErrors.push('Adapter Skip Rows must be greater than or equal to 0 and non-empty.');
        if (settings.MeasurementsPerInterval == null || settings.MeasurementsPerInterval < 0)
            validationErrors.push('Adapter Measurements Per Interval must be greater than 0 and non-empty.');
        props.SetErrors(validationErrors);
    }, [settings === null || settings === void 0 ? void 0 : settings.FramesPerSecond, settings === null || settings === void 0 ? void 0 : settings.FileName, settings === null || settings === void 0 ? void 0 : settings.InputInterval, settings === null || settings === void 0 ? void 0 : settings.SkipRows, settings === null || settings === void 0 ? void 0 : settings.MeasurementsPerInterval, props.SetErrors]);
    const handleSetColumns = react__WEBPACK_IMPORTED_MODULE_0__.useCallback((columns) => {
        setSettings(Object.assign(Object.assign({}, settings), { Columns: columns }));
    }, [settings, setSettings]);
    const handleSetOutputMeasurements = react__WEBPACK_IMPORTED_MODULE_0__.useCallback((measurements) => {
        setSettings(Object.assign(Object.assign({}, settings), { OutputMeasurements: measurements }));
    }, [settings, setSettings]);
    return (react__WEBPACK_IMPORTED_MODULE_0__.createElement(react__WEBPACK_IMPORTED_MODULE_0__.Fragment, null, settings != null ?
        react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'container-fluid d-flex flex-column h-100 p-0' },
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-6' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.Input, { Type: "number", AllowNull: false, Record: settings, Field: "FramesPerSecond", Label: "Frames Per Second", Valid: (field) => settings[field] > 0 && settings[field] < 120, Setter: setSettings, Feedback: 'Adapter Frames Per Second must be greater than 0, less than 120 and non-empty.', Help: (_a = getHelperText('FramesPerSecond', props.ConnectionParameters)) !== null && _a !== void 0 ? _a : undefined })),
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-6' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.Input, { Record: settings, Type: "text", Field: "FileName", Label: "File Name", AllowNull: false, Valid: (field) => { var _a; return settings[field] != null && ((_a = settings[field]) === null || _a === void 0 ? void 0 : _a.length) >= 3; }, Setter: setSettings, Feedback: 'Adapter File Name must be greater than 3 characters and non-empty.', Help: (_b = getHelperText('FileName', props.ConnectionParameters)) !== null && _b !== void 0 ? _b : undefined }))),
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-6' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.Input, { Record: settings, AllowNull: false, Type: "number", Field: "InputInterval", Label: "Input Interval (ms)", Valid: (field) => settings[field] != null, Setter: setSettings, Feedback: 'Adapter Input Interval must be non-empty.', Help: (_c = getHelperText('InputInterval', props.ConnectionParameters)) !== null && _c !== void 0 ? _c : undefined })),
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-6' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.Input, { Record: settings, Type: "number", AllowNull: false, Field: "SkipRows", Label: "Skip Rows", Valid: (field) => settings[field] >= 0, Setter: setSettings, Feedback: 'Adapter Skip Rows must be greater than or equal to 0 and non-empty.', Help: (_d = getHelperText('SkipRows', props.ConnectionParameters)) !== null && _d !== void 0 ? _d : undefined }))),
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row align-items-center' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-6' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.Input, { Record: settings, AllowNull: false, Type: "number", Disabled: !settings.TransverseMode, Field: "MeasurementsPerInterval", Label: "Measurements Per Interval", Valid: (field) => settings[field] > 0, Setter: setSettings, Feedback: 'Adapter Measurements Per Interval must be greater than 0 and non-empty.', Help: (_e = getHelperText('MeasurementsPerInterval', props.ConnectionParameters)) !== null && _e !== void 0 ? _e : undefined }),
                    "                        "),
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-6' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.ToggleSwitch, { Record: settings, Field: "TransverseMode", Label: "Transverse Mode", Setter: setSettings, Help: (_f = getHelperText('TransverseMode', props.ConnectionParameters)) !== null && _f !== void 0 ? _f : undefined }))),
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-4' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.ToggleSwitch, { Record: settings, Field: "SimulateTimestamp", Label: "Simulate Timestamp", Setter: setSettings, Help: (_g = getHelperText('SimulateTimestamp', props.ConnectionParameters)) !== null && _g !== void 0 ? _g : undefined })),
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-4' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.ToggleSwitch, { Record: settings, Field: "AutoRepeat", Label: "Auto Repeat", Setter: setSettings, Help: (_h = getHelperText('AutoRepeat', props.ConnectionParameters)) !== null && _h !== void 0 ? _h : undefined })),
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-4' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.ToggleSwitch, { Record: settings, Field: "UseHighResTimer", Label: "Use High Res Timer", Setter: setSettings, Help: (_j = getHelperText('UseHighResTimer', props.ConnectionParameters)) !== null && _j !== void 0 ? _j : undefined }))),
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_MeasurementSelector__WEBPACK_IMPORTED_MODULE_3__["default"], { SelectedPointTags: settings.OutputMeasurements, SetSelectedPointTags: handleSetOutputMeasurements, GrafanaQueryFuncs: props.GrafanaQueryFuncs }))),
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_ColumnSelector__WEBPACK_IMPORTED_MODULE_4__["default"], { SelectedPointTags: settings.OutputMeasurements, Columns: settings.Columns, SetColumns: handleSetColumns }))))
        : null));
};
// Helper functions
const getHelperText = (fieldName, connectionParameters) => {
    const param = connectionParameters.find(p => p.Name === fieldName);
    if ((param === null || param === void 0 ? void 0 : param.Description) == null || (param === null || param === void 0 ? void 0 : param.Description) === '')
        return null;
    return param.Description;
};
const parseNumber = (value) => {
    const num = Number(value);
    return num;
};
const parseBoolean = (value) => value.toLowerCase() === 'true';
const parseColumns = (value) => {
    const columns = [];
    if (value == null)
        return columns;
    const columnEntries = value.split(';');
    for (let i = 0; i < columnEntries.length; i++) {
        // Expects format 'Column=OutputMeasurement'
        const entry = columnEntries[i].trim();
        const [column, outputMeasurement] = entry.split('=').map(s => s.trim());
        if (isNaN(parseInt(column)) || outputMeasurement == null)
            continue;
        columns.push({ Column: parseInt(column), OutputMeasurement: outputMeasurement });
    }
    return columns;
};
const parseOutputMeasurements = (value) => {
    if (value == null || value === '')
        return [];
    return value
        .split(';')
        .map((measurement, i) => measurement.trim())
        .filter((measurement) => measurement !== '')
        .map((measurement, i) => ({ Index: i, OutputMeasurement: measurement }));
};
const convertParametersToSettings = (params) => {
    var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l;
    const settings = {};
    for (const param of params) {
        const valueStr = param.Value != null && param.Value !== '' ? param.Value : param.DefaultValue;
        const name = param.Name;
        switch (name) {
            case 'FramesPerSecond':
                settings.FramesPerSecond = parseNumber(valueStr);
                break;
            case 'FileName':
                settings.FileName = valueStr;
                break;
            case 'InputInterval':
                settings.InputInterval = parseNumber(valueStr);
                break;
            case 'SkipRows':
                settings.SkipRows = parseNumber(valueStr);
                break;
            case 'TransverseMode':
                settings.TransverseMode = parseBoolean(valueStr);
                break;
            case 'MeasurementsPerInterval':
                settings.MeasurementsPerInterval = parseNumber(valueStr);
                break;
            case 'SimulateTimestamp':
                settings.SimulateTimestamp = parseBoolean(valueStr);
                break;
            case 'AutoRepeat':
                settings.AutoRepeat = parseBoolean(valueStr);
                break;
            case 'UseHighResTimer':
                settings.UseHighResTimer = parseBoolean(valueStr);
                break;
            case 'Columns':
                settings.Columns = parseColumns(valueStr);
                break;
            case 'OutputMeasurements':
                settings.OutputMeasurements = parseOutputMeasurements(valueStr);
                break;
            default:
                break;
        }
    }
    return {
        FramesPerSecond: (_a = settings.FramesPerSecond) !== null && _a !== void 0 ? _a : 0,
        FileName: (_b = settings.FileName) !== null && _b !== void 0 ? _b : '',
        InputInterval: (_c = settings.InputInterval) !== null && _c !== void 0 ? _c : 0,
        SkipRows: (_d = settings.SkipRows) !== null && _d !== void 0 ? _d : 0,
        TransverseMode: (_e = settings.TransverseMode) !== null && _e !== void 0 ? _e : false,
        MeasurementsPerInterval: (_f = settings.MeasurementsPerInterval) !== null && _f !== void 0 ? _f : 0,
        SimulateTimestamp: (_g = settings.SimulateTimestamp) !== null && _g !== void 0 ? _g : false,
        AutoRepeat: (_h = settings.AutoRepeat) !== null && _h !== void 0 ? _h : false,
        UseHighResTimer: (_j = settings.UseHighResTimer) !== null && _j !== void 0 ? _j : false,
        Columns: (_k = settings.Columns) !== null && _k !== void 0 ? _k : [],
        OutputMeasurements: (_l = settings.OutputMeasurements) !== null && _l !== void 0 ? _l : []
    };
};
const sortConParams = (connParams) => {
    return lodash__WEBPACK_IMPORTED_MODULE_2___default().sortBy(connParams, 'Name');
};
const convertSettingsToParameters = (settings, conParams) => {
    const params = [...conParams];
    const newParams = params.map(param => {
        let valueStr = null;
        switch (param.Name) {
            case 'FramesPerSecond':
                valueStr = settings.FramesPerSecond.toString();
                break;
            case 'FileName':
                valueStr = settings.FileName;
                break;
            case 'InputInterval':
                valueStr = settings.InputInterval.toString();
                break;
            case 'SkipRows':
                valueStr = settings.SkipRows.toString();
                break;
            case 'TransverseMode':
                valueStr = settings.TransverseMode.toString();
                break;
            case 'MeasurementsPerInterval':
                valueStr = settings.MeasurementsPerInterval.toString();
                break;
            case 'SimulateTimestamp':
                valueStr = settings.SimulateTimestamp.toString();
                break;
            case 'AutoRepeat':
                valueStr = settings.AutoRepeat.toString();
                break;
            case 'UseHighResTimer':
                valueStr = settings.UseHighResTimer.toString();
                break;
            case 'Columns':
                valueStr = settings.Columns.map(col => `${col.Column}=${col.OutputMeasurement}`).join(';');
                if (valueStr && valueStr[valueStr.length - 1] !== ';')
                    valueStr += ';';
                break;
            case 'OutputMeasurements':
                valueStr = settings.OutputMeasurements.map(meas => `${meas}`).join(';');
                if (valueStr && valueStr[valueStr.length - 1] !== ';')
                    valueStr += ';';
            default:
                valueStr = param.Value;
                break;
        }
        return Object.assign(Object.assign({}, param), { Value: valueStr });
    });
    return newParams;
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (CSVInputAdapterSettingsUI);


/***/ }),

/***/ "./src/CSVInputAdapter/Settings/ColumnSelector.tsx":
/*!*********************************************************!*\
  !*** ./src/CSVInputAdapter/Settings/ColumnSelector.tsx ***!
  \*********************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @gpa-gemstone/react-forms */ "webpack/sharing/consume/default/@gpa-gemstone/react-forms/@gpa-gemstone/react-forms");
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var _gpa_gemstone_react_interactive__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @gpa-gemstone/react-interactive */ "webpack/sharing/consume/default/@gpa-gemstone/react-interactive/@gpa-gemstone/react-interactive");
/* harmony import */ var _gpa_gemstone_react_interactive__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_react_interactive__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @gpa-gemstone/gpa-symbols */ "webpack/sharing/consume/default/@gpa-gemstone/gpa-symbols/@gpa-gemstone/gpa-symbols");
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_3___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_3__);




const ColumnSelector = (props) => {
    const [showAlreadySelectedAlarm, setShowAlreadySelectedAlarm] = react__WEBPACK_IMPORTED_MODULE_0__.useState(false);
    const [showCantAddColAlarm, setShowCantAddColAlarm] = react__WEBPACK_IMPORTED_MODULE_0__.useState(false);
    const measurementOptions = react__WEBPACK_IMPORTED_MODULE_0__.useMemo(() => {
        const options = props.SelectedPointTags.map(meas => ({ Label: meas.OutputMeasurement, Value: meas.OutputMeasurement }));
        options.push({ Label: 'Timestamps', Value: 'Timestamps' });
        return options;
    }, [props.SelectedPointTags]);
    const handleAddRow = react__WEBPACK_IMPORTED_MODULE_0__.useCallback(() => {
        if (props.SelectedPointTags.length === 0)
            return;
        const usedMeasurements = props.Columns.map(col => col.OutputMeasurement);
        const availableMeasurement = props.SelectedPointTags.find(tag => !usedMeasurements.includes(tag.OutputMeasurement));
        if (availableMeasurement != null) {
            const columns = [...props.Columns, { Column: props.Columns.length, OutputMeasurement: availableMeasurement.OutputMeasurement }];
            props.SetColumns(columns);
        }
        else
            setShowCantAddColAlarm(true);
    }, [props.Columns, props.SetColumns, props.SelectedPointTags]);
    const handleSetCol = react__WEBPACK_IMPORTED_MODULE_0__.useCallback((updatedCol, index) => {
        const usedMeasurements = props.Columns.filter((_, i) => i !== index).map(col => col.OutputMeasurement);
        if (usedMeasurements.includes(updatedCol.OutputMeasurement)) {
            setShowAlreadySelectedAlarm(true);
            return;
        }
        const columns = [...props.Columns];
        columns[index] = updatedCol;
        props.SetColumns(columns);
    }, [props.Columns, props.SetColumns]);
    const handleDeleteCol = react__WEBPACK_IMPORTED_MODULE_0__.useCallback((index) => {
        const columns = [...props.Columns];
        columns.splice(index, 1);
        columns.forEach((col, idx) => col.Column = idx);
        props.SetColumns(columns);
    }, [props.Columns, props.SetColumns]);
    return (react__WEBPACK_IMPORTED_MODULE_0__.createElement(react__WEBPACK_IMPORTED_MODULE_0__.Fragment, null,
        react__WEBPACK_IMPORTED_MODULE_0__.createElement("fieldset", { className: "border", style: { padding: '10px' } },
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("legend", { style: { fontSize: 'large' } }, "Column Selector"),
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12' },
                    props.Columns.map((col, i) => (react__WEBPACK_IMPORTED_MODULE_0__.createElement(react__WEBPACK_IMPORTED_MODULE_0__.Fragment, { key: i },
                        react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row align-items-center' },
                            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-auto' },
                                react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.Input, { Record: col, Field: "Column", Label: 'Column', Type: "number", AllowNull: false, Setter: (record) => handleSetCol(record, i), Valid: (field) => col[field] >= 0, Feedback: 'Column must be greater than or equal to 0 and non-empty.' })),
                            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col' },
                                react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.Select, { Record: col, Field: 'OutputMeasurement', Label: "Measurement", Setter: (record) => handleSetCol(record, i), Options: measurementOptions })),
                            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-auto' },
                                react__WEBPACK_IMPORTED_MODULE_0__.createElement("button", { className: 'btn', onClick: () => handleDeleteCol(i) },
                                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_3__.ReactIcons.TrashCan, { Color: 'red' })))),
                        react__WEBPACK_IMPORTED_MODULE_0__.createElement("hr", { className: 'm-0' }),
                        i === props.Columns.length - 1 ?
                            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12' },
                                    react__WEBPACK_IMPORTED_MODULE_0__.createElement("button", { className: 'btn btn-primary', onClick: () => handleAddRow() }, "Add New Column")))
                            : null))),
                    props.Columns.length === 0 && props.SelectedPointTags.length !== 0 ?
                        react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12' },
                                react__WEBPACK_IMPORTED_MODULE_0__.createElement("button", { className: 'btn btn-primary', onClick: () => handleAddRow() }, "Add New Column")))
                        : null)),
            react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_interactive__WEBPACK_IMPORTED_MODULE_2__.Alert, { Show: showCantAddColAlarm, SetShow: setShowCantAddColAlarm, AlertColor: 'alert-info' }, "Unable to add column: All available measurements have already been selected."),
            react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_interactive__WEBPACK_IMPORTED_MODULE_2__.Alert, { Show: showAlreadySelectedAlarm, SetShow: setShowAlreadySelectedAlarm, AlertColor: 'alert-info' }, "The selected output measurement is already in use by another column. Please select another one."))));
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (ColumnSelector);


/***/ }),

/***/ "./src/CSVInputAdapter/Settings/MeasurementSelector.tsx":
/*!**************************************************************!*\
  !*** ./src/CSVInputAdapter/Settings/MeasurementSelector.tsx ***!
  \**************************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @gpa-gemstone/react-forms */ "webpack/sharing/consume/default/@gpa-gemstone/react-forms/@gpa-gemstone/react-forms");
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @gpa-gemstone/gpa-symbols */ "webpack/sharing/consume/default/@gpa-gemstone/gpa-symbols/@gpa-gemstone/gpa-symbols");
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var _PointTagFilter__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./PointTagFilter */ "./src/CSVInputAdapter/Settings/PointTagFilter.tsx");

//import { GrafanaQueryFunctions } from "../../GrafanaQueryFunctions";



const MeasurementSelector = (props) => {
    var _a, _b;
    const handlePointTagSearch = react__WEBPACK_IMPORTED_MODULE_0__.useCallback((searchText) => {
        let options = [];
        const handle = props.GrafanaQueryFuncs.Search({
            dataTypeIndex: 0,
            expression: `SELECT PointTag FROM ActiveMeasurement WHERE PointTag LIKE %${searchText}%`
        });
        const promise = new Promise((resolve, reject) => {
            handle.done((data) => {
                options = data.map(dd => {
                    return { Label: `${dd}`, Value: `${dd}` };
                });
                options.push({ Label: 'FILTER ActiveMeasurements WHERE', Value: `FILTER ALL ActiveMeasurements WHERE SignalType = 'Freq'` });
                resolve(options);
            }).fail((error) => {
                reject(error);
            });
        });
        return [promise, () => handle.abort()];
    }, [props.GrafanaQueryFuncs]);
    const handleDeletePointTag = react__WEBPACK_IMPORTED_MODULE_0__.useCallback((tag) => {
        let pointTags = [...props.SelectedPointTags].filter(t => t.Index !== tag.Index);
        props.SetSelectedPointTags(pointTags);
    }, [props.SelectedPointTags, props.SetSelectedPointTags]);
    const handleSetPointTag = react__WEBPACK_IMPORTED_MODULE_0__.useCallback((meas) => {
        let pointTags = [...props.SelectedPointTags];
        const maxIndex = pointTags.length > 0 ? Math.max(...pointTags.map(tag => tag.Index)) : -1;
        const selectedMeas = Object.assign(Object.assign({}, meas), { Index: maxIndex + 1 });
        if (!pointTags.map(t => t.OutputMeasurement).includes(selectedMeas.OutputMeasurement))
            pointTags.push(selectedMeas);
        props.SetSelectedPointTags(pointTags);
    }, [props.SelectedPointTags, props.SetSelectedPointTags]);
    const handleSetFilter = react__WEBPACK_IMPORTED_MODULE_0__.useCallback((outputMeasurement) => {
        const updatedMeasurements = props.SelectedPointTags.map((measurement, i) => {
            if (measurement.Index === outputMeasurement.Index)
                return Object.assign(Object.assign({}, measurement), { OutputMeasurement: outputMeasurement.OutputMeasurement });
            return measurement;
        });
        props.SetSelectedPointTags(updatedMeasurements);
    }, [props.SelectedPointTags, props.SetSelectedPointTags]);
    return (react__WEBPACK_IMPORTED_MODULE_0__.createElement(react__WEBPACK_IMPORTED_MODULE_0__.Fragment, null,
        react__WEBPACK_IMPORTED_MODULE_0__.createElement("fieldset", { className: "border", style: { padding: '10px' } },
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("legend", { style: { fontSize: 'large' } }, "Output Measurement Selector"),
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                        react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12' },
                            react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.SearchableSelect, { Search: handlePointTagSearch, Record: (_b = (_a = props.SelectedPointTags) === null || _a === void 0 ? void 0 : _a[props.SelectedPointTags.length - 1]) !== null && _b !== void 0 ? _b : { OutputMeasurement: '', Index: 0 }, Field: 'OutputMeasurement', Setter: handleSetPointTag, Style: { width: '100%' }, BtnStyle: { display: 'flex', paddingLeft: 0, alignItems: 'center' }, AllowCustom: false, Label: "Output Measurement", SearchLabel: '' }))),
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                        react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12' }, props.SelectedPointTags.filter(tag => !tag.OutputMeasurement.includes('FILTER')).map((tag, i) => (react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { key: i, className: 'badge badge-pill badge-info m-2', style: { fontSize: '100%' } },
                            react__WEBPACK_IMPORTED_MODULE_0__.createElement("span", null, tag.OutputMeasurement),
                            react__WEBPACK_IMPORTED_MODULE_0__.createElement("button", { className: 'btn', onClick: () => handleDeletePointTag(tag) },
                                react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2__.ReactIcons.TrashCan, { Color: 'red' }))))))),
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement("hr", null),
                    props.SelectedPointTags.filter(tag => tag.OutputMeasurement.includes('FILTER')).map((tag, i) => (react__WEBPACK_IMPORTED_MODULE_0__.createElement(_PointTagFilter__WEBPACK_IMPORTED_MODULE_3__["default"], { key: i, OutputMeasurement: tag, SetOutputMeasurement: handleSetFilter, DeleteOutputMeasurement: handleDeletePointTag }))))))));
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (MeasurementSelector);


/***/ }),

/***/ "./src/CSVInputAdapter/Settings/PointTagFilter.tsx":
/*!*********************************************************!*\
  !*** ./src/CSVInputAdapter/Settings/PointTagFilter.tsx ***!
  \*********************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @gpa-gemstone/react-forms */ "webpack/sharing/consume/default/@gpa-gemstone/react-forms/@gpa-gemstone/react-forms");
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @gpa-gemstone/gpa-symbols */ "webpack/sharing/consume/default/@gpa-gemstone/gpa-symbols/@gpa-gemstone/gpa-symbols");
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2__);



const limitClauseOptions = [
    { Label: "TOP", Value: "TOP" },
    { Label: "ALL", Value: "ALL" },
    { Label: "BOTTOM", Value: "BOTTOM" }
];
const defaultFilter = {
    WhereClause: `SignalType = 'Freq'`,
    Limit: 0,
    LimitClause: 'ALL'
};
const PointTagFilter = (props) => {
    const [filter, setFilter] = react__WEBPACK_IMPORTED_MODULE_0__.useState(parseFilter(props.OutputMeasurement.OutputMeasurement));
    react__WEBPACK_IMPORTED_MODULE_0__.useEffect(() => {
        let outputMeasurement = `FILTER ${filter.LimitClause}`;
        if (filter.LimitClause !== 'ALL')
            outputMeasurement += ` ${filter.Limit}`;
        outputMeasurement += ` ActiveMeasurements WHERE ${filter.WhereClause}`;
        props.SetOutputMeasurement(Object.assign(Object.assign({}, props.OutputMeasurement), { OutputMeasurement: outputMeasurement }));
    }, [filter]);
    return (react__WEBPACK_IMPORTED_MODULE_0__.createElement(react__WEBPACK_IMPORTED_MODULE_0__.Fragment, null, filter !== null ?
        react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-auto p-0' },
                        react__WEBPACK_IMPORTED_MODULE_0__.createElement("button", { className: 'btn pr-0', onClick: () => props.DeleteOutputMeasurement(props.OutputMeasurement) },
                            react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2__.ReactIcons.TrashCan, { Color: 'red' }))),
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-auto p-0' },
                        react__WEBPACK_IMPORTED_MODULE_0__.createElement("button", { className: 'btn pl-0 pr-0' },
                            "Filter ",
                            react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2__.ReactIcons.Filter, null))),
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-auto p-0' },
                        react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.Select, { Record: filter, Field: "LimitClause", Label: "", Setter: setFilter, Options: limitClauseOptions })),
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-auto p-0' },
                        react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.Input, { Disabled: filter.LimitClause === 'ALL', Record: filter, Setter: setFilter, Field: "Limit", Label: "", Type: "number", Valid: (field) => filter.LimitClause === 'ALL' || filter[field] > 0, Feedback: 'Limit must be greater than 0.' })),
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col' },
                        react__WEBPACK_IMPORTED_MODULE_0__.createElement("label", null, "Active Measurements"))),
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row align-items-center' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-auto pr-1' },
                        react__WEBPACK_IMPORTED_MODULE_0__.createElement("label", null, "WHERE")),
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col p-0' },
                        react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.Input, { Record: filter, Setter: setFilter, Field: "WhereClause", Label: "", Type: "text", Valid: (field) => filter[field] != null, Feedback: 'Where Clause can not be empty.' })))))
        : null));
};
const parseFilter = (outputMeasurement) => {
    const regex = /^FILTER\s+(TOP|BOTTOM|ALL)(?:\s+(\d+))?\s+ActiveMeasurements\s+WHERE\s+(.*)$/i;
    const match = outputMeasurement.match(regex);
    if (match != null) {
        const limitClause = match[1].toUpperCase();
        const limit = match[2] ? parseInt(match[2], 10) : 0;
        const whereClause = match[3].trim();
        return {
            LimitClause: limitClause,
            Limit: limit,
            WhereClause: whereClause
        };
    }
    return defaultFilter;
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (PointTagFilter);


/***/ })

}]);