"use strict";
(self["webpackChunk"] = self["webpackChunk"] || []).push([["openHistorian_Adapters_CsvInputAdapter_tsx"],{

/***/ "./CSVSettings.tsx":
/*!*************************!*\
  !*** ./CSVSettings.tsx ***!
  \*************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?8067");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @gpa-gemstone/react-forms */ "webpack/sharing/consume/default/@gpa-gemstone/react-forms/@gpa-gemstone/react-forms?4f75");
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var lodash__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! lodash */ "webpack/sharing/consume/default/lodash/lodash?5032");
/* harmony import */ var lodash__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(lodash__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var _Common_TSX_Adapters_MeasurementSelector__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../../../Common/TSX/Adapters/MeasurementSelector */ "../../../../Common/TSX/Adapters/MeasurementSelector.tsx");
/* harmony import */ var _Common_TSX_Adapters_ColumnSelector__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../../../Common/TSX/Adapters/ColumnSelector */ "../../../../Common/TSX/Adapters/ColumnSelector.tsx");
/* harmony import */ var _Common_TSX_Adapters_HelperFunctions__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../../../Common/TSX/Adapters/HelperFunctions */ "../../../../Common/TSX/Adapters/HelperFunctions.tsx");
//******************************************************************************************************
//  CSVSettings.tsx - Gbtc
//
//  Copyright © 2024, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA may license this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  11/19/2024 - Preston Crawford
//       Generated original version of source code.
//
//******************************************************************************************************






const CSVInputAdapterSettingsUI = (props) => {
    const [settings, setSettings] = react__WEBPACK_IMPORTED_MODULE_0__.useState(null);
    react__WEBPACK_IMPORTED_MODULE_0__.useEffect(() => {
        setSettings((0,_Common_TSX_Adapters_HelperFunctions__WEBPACK_IMPORTED_MODULE_5__.convertParametersToSettings)(props.ConnectionParameters));
    }, [props.ConnectionParameters]);
    react__WEBPACK_IMPORTED_MODULE_0__.useEffect(() => {
        if (settings == null)
            return;
        const newParams = (0,_Common_TSX_Adapters_HelperFunctions__WEBPACK_IMPORTED_MODULE_5__.convertSettingsToParameters)(settings, props.ConnectionParameters);
        const sortedNewParams = (0,_Common_TSX_Adapters_HelperFunctions__WEBPACK_IMPORTED_MODULE_5__.sortConParams)(newParams);
        const sortedCurrentParams = (0,_Common_TSX_Adapters_HelperFunctions__WEBPACK_IMPORTED_MODULE_5__.sortConParams)(props.ConnectionParameters);
        if (!lodash__WEBPACK_IMPORTED_MODULE_2___default().isEqual(sortedNewParams, sortedCurrentParams))
            props.SetConnectionParameters(newParams);
    }, [settings, props.SetConnectionParameters, props.ConnectionParameters]);
    react__WEBPACK_IMPORTED_MODULE_0__.useEffect(() => {
        const validationErrors = [];
        if (settings == null)
            return;
        if (settings.FramesPerSecond == null || settings.FramesPerSecond <= 0 || settings.FramesPerSecond > 120)
            validationErrors.push('Adapter Frames Per Second must be greater than 0, less than 120 and non-empty.');
        if (settings.FileName == null || settings.FileName.length < 3)
            validationErrors.push('Adapter File Name must be greater than 3 characters and non-empty.');
        if (settings.InputInterval == null)
            validationErrors.push('Adapter Input Interval must be non-empty.');
        if (settings.SkipRows == null || settings.SkipRows < 0)
            validationErrors.push('Adapter Skip Rows must be greater than or equal to 0 and non-empty.');
        if (settings.MeasurementsPerInterval == null || settings.MeasurementsPerInterval <= 0)
            validationErrors.push('Adapter Measurements Per Interval must be greater than 0 and non-empty.');
        props.SetErrors(validationErrors);
    }, [settings === null || settings === void 0 ? void 0 : settings.FramesPerSecond, settings === null || settings === void 0 ? void 0 : settings.FileName, settings === null || settings === void 0 ? void 0 : settings.InputInterval, settings === null || settings === void 0 ? void 0 : settings.SkipRows, settings === null || settings === void 0 ? void 0 : settings.MeasurementsPerInterval, props.SetErrors]);
    const handleSetColumns = react__WEBPACK_IMPORTED_MODULE_0__.useCallback((columns) => {
        setSettings(Object.assign(Object.assign({}, settings), { ColumnMappings: columns }));
    }, [settings, setSettings]);
    const handleSetOutputMeasurements = react__WEBPACK_IMPORTED_MODULE_0__.useCallback((measurements) => {
        setSettings(Object.assign(Object.assign({}, settings), { OutputMeasurements: measurements }));
    }, [settings, setSettings]);
    return (react__WEBPACK_IMPORTED_MODULE_0__.createElement(react__WEBPACK_IMPORTED_MODULE_0__.Fragment, null, settings != null ?
        react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'container-fluid d-flex flex-column h-100 p-0' },
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-6' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.Input, { Type: "number", AllowNull: false, Record: settings, Field: "FramesPerSecond", Label: "Frames Per Second", Valid: (field) => settings[field] > 0 && settings[field] < 120, Setter: setSettings, Feedback: 'Adapter Frames Per Second must be greater than 0, less than 120 and non-empty.', Help: (0,_Common_TSX_Adapters_HelperFunctions__WEBPACK_IMPORTED_MODULE_5__.getHelperText)('FramesPerSecond', props.ConnectionParameters) })),
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-6' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.Input, { Record: settings, Type: "text", Field: "FileName", Label: "File Name", AllowNull: false, Valid: (field) => { var _a; return settings[field] != null && ((_a = settings[field]) === null || _a === void 0 ? void 0 : _a.length) >= 3; }, Setter: setSettings, Feedback: 'Adapter File Name must be greater than 3 characters and non-empty.', Help: (0,_Common_TSX_Adapters_HelperFunctions__WEBPACK_IMPORTED_MODULE_5__.getHelperText)('FileName', props.ConnectionParameters) }))),
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-6' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.Input, { Record: settings, AllowNull: false, Type: "number", Field: "InputInterval", Label: "Input Interval (ms)", Valid: (field) => settings[field] != null, Setter: setSettings, Feedback: 'Adapter Input Interval must be non-empty.', Help: (0,_Common_TSX_Adapters_HelperFunctions__WEBPACK_IMPORTED_MODULE_5__.getHelperText)('InputInterval', props.ConnectionParameters) })),
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-6' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.Input, { Record: settings, Type: "number", AllowNull: false, Field: "SkipRows", Label: "Skip Rows", Valid: (field) => settings[field] >= 0, Setter: setSettings, Feedback: 'Adapter Skip Rows must be greater than or equal to 0 and non-empty.', Help: (0,_Common_TSX_Adapters_HelperFunctions__WEBPACK_IMPORTED_MODULE_5__.getHelperText)('SkipRows', props.ConnectionParameters) }))),
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row align-items-center' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-6' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.Input, { Record: settings, AllowNull: false, Type: "number", Disabled: !settings.TransverseMode, Field: "MeasurementsPerInterval", Label: "Measurements Per Interval", Valid: (field) => settings[field] > 0, Setter: setSettings, Feedback: 'Adapter Measurements Per Interval must be greater than 0 and non-empty.', Help: (0,_Common_TSX_Adapters_HelperFunctions__WEBPACK_IMPORTED_MODULE_5__.getHelperText)('MeasurementsPerInterval', props.ConnectionParameters) }),
                    "                        "),
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-6' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.ToggleSwitch, { Record: settings, Field: "TransverseMode", Label: "Transverse Mode", Setter: setSettings, Help: (0,_Common_TSX_Adapters_HelperFunctions__WEBPACK_IMPORTED_MODULE_5__.getHelperText)('TransverseMode', props.ConnectionParameters) }))),
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-4' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.ToggleSwitch, { Record: settings, Field: "SimulateTimestamp", Label: "Simulate Timestamp", Setter: setSettings, Help: (0,_Common_TSX_Adapters_HelperFunctions__WEBPACK_IMPORTED_MODULE_5__.getHelperText)('SimulateTimestamp', props.ConnectionParameters) })),
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-4' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.ToggleSwitch, { Record: settings, Field: "AutoRepeat", Label: "Auto Repeat", Setter: setSettings, Help: (0,_Common_TSX_Adapters_HelperFunctions__WEBPACK_IMPORTED_MODULE_5__.getHelperText)('AutoRepeat', props.ConnectionParameters) })),
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-4' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.ToggleSwitch, { Record: settings, Field: "UseHighResTimer", Label: "Use High Res Timer", Setter: setSettings, Help: (0,_Common_TSX_Adapters_HelperFunctions__WEBPACK_IMPORTED_MODULE_5__.getHelperText)('UseHighResTimer', props.ConnectionParameters) }))),
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_Common_TSX_Adapters_MeasurementSelector__WEBPACK_IMPORTED_MODULE_3__["default"], { SelectedPointTags: settings.OutputMeasurements, SetSelectedPointTags: handleSetOutputMeasurements, HomePath: props.HomePath, Type: 'Output' }))),
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_Common_TSX_Adapters_ColumnSelector__WEBPACK_IMPORTED_MODULE_4__["default"], { SelectedPointTags: settings.OutputMeasurements, Columns: settings.ColumnMappings, SetColumns: handleSetColumns }))))
        : null));
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (CSVInputAdapterSettingsUI);


/***/ }),

/***/ "./openHistorian_Adapters_CsvInputAdapter.tsx":
/*!****************************************************!*\
  !*** ./openHistorian_Adapters_CsvInputAdapter.tsx ***!
  \****************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var _CSVSettings__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./CSVSettings */ "./CSVSettings.tsx");
//******************************************************************************************************
//  CSVInputAdapter.tsx - Gbtc
//
//  Copyright © 2024, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA may license this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  11/19/2024 - Preston Crawford
//       Generated original version of source code.
//
//******************************************************************************************************

const CSVInputAdapter = {
    Settings: _CSVSettings__WEBPACK_IMPORTED_MODULE_0__["default"],
    Output: null //will use the default UI
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (CSVInputAdapter);


/***/ }),

/***/ "../../../../Common/TSX/Adapters/ColumnSelector.tsx":
/*!**********************************************************!*\
  !*** ../../../../Common/TSX/Adapters/ColumnSelector.tsx ***!
  \**********************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?8d15");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @gpa-gemstone/react-forms */ "webpack/sharing/consume/default/@gpa-gemstone/react-forms/@gpa-gemstone/react-forms?d6ca");
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var _gpa_gemstone_react_interactive__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @gpa-gemstone/react-interactive */ "webpack/sharing/consume/default/@gpa-gemstone/react-interactive/@gpa-gemstone/react-interactive");
/* harmony import */ var _gpa_gemstone_react_interactive__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_react_interactive__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @gpa-gemstone/gpa-symbols */ "webpack/sharing/consume/default/@gpa-gemstone/gpa-symbols/@gpa-gemstone/gpa-symbols?69ff");
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_3___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_3__);
//******************************************************************************************************
//  ColumnSelector.tsx - Gbtc
//
//  Copyright © 2024, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA may license this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  11/19/2024 - Preston Crawford
//       Generated original version of source code.
//
//******************************************************************************************************




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

/***/ "../../../../Common/TSX/Adapters/HelperFunctions.tsx":
/*!***********************************************************!*\
  !*** ../../../../Common/TSX/Adapters/HelperFunctions.tsx ***!
  \***********************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   convertParametersToSettings: () => (/* binding */ convertParametersToSettings),
/* harmony export */   convertSettingsToParameters: () => (/* binding */ convertSettingsToParameters),
/* harmony export */   getHelperText: () => (/* binding */ getHelperText),
/* harmony export */   handleDeviceIDSearch: () => (/* binding */ handleDeviceIDSearch),
/* harmony export */   handleHistorianAcronymSearch: () => (/* binding */ handleHistorianAcronymSearch),
/* harmony export */   handleHistorianSearch: () => (/* binding */ handleHistorianSearch),
/* harmony export */   handleTimeFormatSearch: () => (/* binding */ handleTimeFormatSearch),
/* harmony export */   isISettingsKey: () => (/* binding */ isISettingsKey),
/* harmony export */   parseBoolean: () => (/* binding */ parseBoolean),
/* harmony export */   parseCsvVal: () => (/* binding */ parseCsvVal),
/* harmony export */   parseNumber: () => (/* binding */ parseNumber),
/* harmony export */   parseOutputMeasurements: () => (/* binding */ parseOutputMeasurements),
/* harmony export */   parseSemicolanVal: () => (/* binding */ parseSemicolanVal),
/* harmony export */   sortConParams: () => (/* binding */ sortConParams)
/* harmony export */ });
/* harmony import */ var _global__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../global */ "../../../../Common/TSX/global.ts");
/* harmony import */ var lodash__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! lodash */ "webpack/sharing/consume/default/lodash/lodash?4879");
/* harmony import */ var lodash__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(lodash__WEBPACK_IMPORTED_MODULE_1__);
//******************************************************************************************************
//  HelperFunctions.tsx - Gbtc
//
//  Copyright © 2024, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA may license this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  11/19/2024 - Preston Crawford
//       Generated original version of source code.
//
//******************************************************************************************************


const parseNumber = (value) => {
    const num = Number(value);
    return num;
};
const parseBoolean = (value) => value.toLowerCase() === 'true';
const parseOutputMeasurements = (value) => {
    if (value == null || value === '')
        return [];
    return value
        .split(';')
        .map((measurement, i) => measurement.trim())
        .filter((measurement) => measurement !== '')
        .map((measurement, i) => ({ Index: i, OutputMeasurement: measurement }));
};
const parseCsvVal = (value) => {
    if (value == null || value === '')
        return [];
    return value
        .split(',')
        .map((server, i) => server.trim())
        .filter((server) => server !== '');
};
const parseSemicolanVal = (value) => {
    if (value == null || value === '')
        return [];
    return value
        .split(';')
        .map((server, i) => server.trim())
        .filter((server) => server !== '');
};
const sortConParams = (connParams) => {
    return lodash__WEBPACK_IMPORTED_MODULE_1___default().sortBy(connParams, 'Name');
};
const handleTimeFormatSearch = (searchString) => {
    const promise = new Promise((resolve) => {
        const filteredOptions = lodash__WEBPACK_IMPORTED_MODULE_1___default().filter(TimeFormatOptions, (option) => lodash__WEBPACK_IMPORTED_MODULE_1___default().includes(option.Label.toLowerCase(), searchString.toLowerCase()) //label and value are the same in this case
        );
        resolve(filteredOptions);
    });
    return [promise, () => { }];
};
const TimeFormatOptions = [
    { Label: 'YYYY-MM-DDTHH:mm', Value: 'YYYY-MM-DDTHH:mm' }, { Label: 'YYYY-MM-DDTHH:mm:ss', Value: 'YYYY-MM-DDTHH:mm:ss' }, { Label: 'YYYY-MM-DDTHH:mm:ss.SSS', Value: 'YYYY-MM-DDTHH:mm:ss.SSS' },
    { Label: 'YYYY-MM-DD', Value: 'YYYY-MM-DD' }, { Label: 'MM/DD/YYYY', Value: 'MM/DD/YYYY' }, { Label: 'HH:mm', Value: 'HH:mm' }, { Label: 'HH:mm', Value: 'HH:mm' }, { Label: 'HH:mm:ss', Value: 'HH:mm:ss' },
    { Label: 'HH:mm:ss.SSS', Value: 'HH:mm:ss.SSS' }
];
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
function convertSettingsToParameters(settings, conParams) {
    const params = [...conParams];
    const newParams = params.map(param => {
        var _a, _b;
        let valueStr = null;
        if (param.Name === 'OutputMeasurements') {
            valueStr = settings.OutputMeasurements.map(meas => `${meas.OutputMeasurement}`).join(';');
            if (valueStr && valueStr[valueStr.length - 1] !== ';')
                valueStr += ';';
        }
        else if (param.Name === 'ColumnMappings') {
            valueStr = settings.ColumnMappings.map(col => `${col.Column}=${col.OutputMeasurement}`).join(';');
            if (valueStr && valueStr[valueStr.length - 1] !== ';')
                valueStr += ';';
        }
        else if (param.Name === 'Servers' || param.Name === 'CcRecipients' || param.Name === 'BccRecipients' || param.Name === 'ToRecipients' || param.Name === 'SettingsOutputIDs' || param.Name === 'FrequencyTriggers') {
            valueStr = settings.Servers.map(serv => `${serv}`).join(',');
            if (valueStr && valueStr[valueStr.length - 1] !== ',')
                valueStr += ',';
        }
        else if (param.Name === 'MetadataTables') {
            valueStr = settings.Servers.map(serv => `${serv}`).join(';');
            if (valueStr && valueStr[valueStr.length - 1] !== ';')
                valueStr += ';';
        }
        else
            valueStr = (_b = (_a = settings[param.Name]) === null || _a === void 0 ? void 0 : _a.toString()) !== null && _b !== void 0 ? _b : param.Value;
        return Object.assign(Object.assign({}, param), { Value: valueStr });
    });
    return newParams;
}
;
function convertParametersToSettings(params) {
    const settings = {};
    for (const param of params) {
        const valueStr = param.Value !== null && param.Value !== '' ? param.Value : param.DefaultValue;
        const name = param.Name;
        const dataType = param.DataType;
        if (name === 'OutputMeasurements')
            // Handle special parsing for OutputMeasurements
            settings[name] = parseOutputMeasurements(valueStr);
        else if (name === 'ColumnMappings') {
            settings[name] = parseColumns(valueStr);
        }
        else if (name === 'BccRecipients' || name === 'CcRecipients' || name === 'ToRecipients' || name === 'SettingsOutputIDs' || name === 'MetadataTables' || name === 'Servers' || name === 'FrequencyTriggers') {
            settings[name] = parseCsvVal(valueStr);
        }
        else if (name === 'MetadataTables') {
            settings[name] = parseSemicolanVal(valueStr);
        }
        else {
            let parsedValue;
            switch (dataType) {
                case _global__WEBPACK_IMPORTED_MODULE_0__.PhasorProtocolDataType.Int16:
                case _global__WEBPACK_IMPORTED_MODULE_0__.PhasorProtocolDataType.UInt16:
                case _global__WEBPACK_IMPORTED_MODULE_0__.PhasorProtocolDataType.Int32:
                case _global__WEBPACK_IMPORTED_MODULE_0__.PhasorProtocolDataType.UInt32:
                case _global__WEBPACK_IMPORTED_MODULE_0__.PhasorProtocolDataType.Int64:
                case _global__WEBPACK_IMPORTED_MODULE_0__.PhasorProtocolDataType.UInt64:
                case _global__WEBPACK_IMPORTED_MODULE_0__.PhasorProtocolDataType.Single:
                case _global__WEBPACK_IMPORTED_MODULE_0__.PhasorProtocolDataType.Double:
                    parsedValue = parseNumber(valueStr);
                    break;
                case _global__WEBPACK_IMPORTED_MODULE_0__.PhasorProtocolDataType.Boolean:
                    parsedValue = parseBoolean(valueStr);
                    break;
                case _global__WEBPACK_IMPORTED_MODULE_0__.PhasorProtocolDataType.DateTime:
                    parsedValue = new Date(valueStr);
                    break;
                case _global__WEBPACK_IMPORTED_MODULE_0__.PhasorProtocolDataType.String:
                case _global__WEBPACK_IMPORTED_MODULE_0__.PhasorProtocolDataType.Enum:
                default:
                    parsedValue = valueStr;
                    break;
            }
            settings[name] = parsedValue;
        }
    }
    return settings;
}
const isISettingsKey = (key, defaultSettings) => {
    return key in defaultSettings;
};
const getHelperText = (fieldName, connectionParameters) => {
    const param = connectionParameters.find(p => p.Name === fieldName);
    if ((param === null || param === void 0 ? void 0 : param.Description) == null || (param === null || param === void 0 ? void 0 : param.Description) === '')
        return undefined;
    return param.Description;
};
const handleHistorianSearch = (searchString, HistorianQueries) => {
    const filters = [
        { FieldName: 'Name', SearchParameter: `%${searchString}%`, Operator: 'LIKE' },
        { FieldName: 'Acronym', SearchParameter: `%${searchString}%`, Operator: 'LIKE' }
    ];
    let options = [];
    const handle = HistorianQueries.SearchPage(0, 'Name', true, filters);
    const promise = new Promise((resolve, reject) => {
        handle.done((data) => {
            options = data.map(dd => ({ Label: `${dd.Name}`, Value: `${dd.ID}-${dd.Name}` }));
            resolve(options);
        }).fail((error) => {
            reject(error);
        });
    });
    return [promise, () => handle.abort()];
};
const handleHistorianAcronymSearch = (searchString, HistorianQueries) => {
    const filters = [
        { FieldName: 'Name', SearchParameter: `%${searchString}%`, Operator: 'LIKE' },
        { FieldName: 'Acronym', SearchParameter: `%${searchString}%`, Operator: 'LIKE' }
    ];
    let options = [];
    const handle = HistorianQueries.SearchPage(0, 'Name', true, filters);
    const promise = new Promise((resolve, reject) => {
        handle.done((data) => {
            options = data.map(dd => ({ Label: `${dd.Name}`, Value: `${dd.ID}-${dd.Name}` }));
            resolve(options);
        }).fail((error) => {
            reject(error);
        });
    });
    return [promise, () => handle.abort()];
};
const handleDeviceIDSearch = (searchString, deviceQueries) => {
    const filters = [
        { FieldName: 'Name', SearchParameter: `%${searchString}%`, Operator: 'LIKE' },
        { FieldName: 'Acronym', SearchParameter: `%${searchString}%`, Operator: 'LIKE' }
    ];
    let options = [];
    const handle = deviceQueries.SearchPage(0, 'Name', true, filters);
    const promise = new Promise((resolve, reject) => {
        handle.done((data) => {
            options = data.map(dd => ({ Label: `${dd.Name}(${dd.Acronym})`, Value: `${dd.ID}-${dd.Name}` }));
            resolve(options);
        }).fail((error) => {
            reject(error);
        });
    });
    return [promise, () => handle.abort()];
};


/***/ }),

/***/ "../../../../Common/TSX/Adapters/MeasurementSelector.tsx":
/*!***************************************************************!*\
  !*** ../../../../Common/TSX/Adapters/MeasurementSelector.tsx ***!
  \***************************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?8d15");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @gpa-gemstone/react-forms */ "webpack/sharing/consume/default/@gpa-gemstone/react-forms/@gpa-gemstone/react-forms?d6ca");
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @gpa-gemstone/gpa-symbols */ "webpack/sharing/consume/default/@gpa-gemstone/gpa-symbols/@gpa-gemstone/gpa-symbols?69ff");
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var _PointTagFilter__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./PointTagFilter */ "../../../../Common/TSX/Adapters/PointTagFilter.tsx");
/* harmony import */ var _GrafanaQueryFunctions__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../GrafanaQueryFunctions */ "../../../../Common/TSX/GrafanaQueryFunctions.tsx");
//******************************************************************************************************
//  MeasurementSelector.tsx - Gbtc
//
//  Copyright © 2024, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA may license this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  11/19/2024 - Preston Crawford
//       Generated original version of source code.
//
//******************************************************************************************************





const MeasurementSelector = (props) => {
    var _a, _b;
    const GrafanaQueries = new _GrafanaQueryFunctions__WEBPACK_IMPORTED_MODULE_4__.GrafanaQueryFunctions(`${props.HomePath}api/grafana`);
    const handlePointTagSearch = react__WEBPACK_IMPORTED_MODULE_0__.useCallback((searchText) => {
        let options = [];
        const handle = GrafanaQueries.Search({
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
    }, [GrafanaQueries]);
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
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("legend", { style: { fontSize: 'large' } },
                props.Type,
                " Measurement Selector"),
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

/***/ "../../../../Common/TSX/Adapters/PointTagFilter.tsx":
/*!**********************************************************!*\
  !*** ../../../../Common/TSX/Adapters/PointTagFilter.tsx ***!
  \**********************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?8d15");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @gpa-gemstone/react-forms */ "webpack/sharing/consume/default/@gpa-gemstone/react-forms/@gpa-gemstone/react-forms?d6ca");
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @gpa-gemstone/gpa-symbols */ "webpack/sharing/consume/default/@gpa-gemstone/gpa-symbols/@gpa-gemstone/gpa-symbols?69ff");
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2__);
//******************************************************************************************************
//  PointTagFilter.tsx - Gbtc
//
//  Copyright © 2024, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA may license this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  11/19/2024 - Preston Crawford
//       Generated original version of source code.
//
//******************************************************************************************************



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


/***/ }),

/***/ "../../../../Common/TSX/GrafanaQueryFunctions.tsx":
/*!********************************************************!*\
  !*** ../../../../Common/TSX/GrafanaQueryFunctions.tsx ***!
  \********************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   GrafanaQueryFunctions: () => (/* binding */ GrafanaQueryFunctions)
/* harmony export */ });
/* harmony import */ var jquery__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! jquery */ "../../../../Common/node_modules/jquery/dist/jquery.js");
/* harmony import */ var jquery__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(jquery__WEBPACK_IMPORTED_MODULE_0__);

class GrafanaQueryFunctions {
    constructor(GrafanaAPIPath) {
        this.Search = (request) => {
            return jquery__WEBPACK_IMPORTED_MODULE_0___default().ajax({
                type: 'POST',
                url: `${this.GrafanaAPIPath}/Search`,
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                data: JSON.stringify(request),
                cache: false,
                async: true
            });
        };
        this.Annotations = (request) => {
            return jquery__WEBPACK_IMPORTED_MODULE_0___default().ajax({
                type: 'POST',
                url: `${this.GrafanaAPIPath}/Annotations`,
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                data: JSON.stringify(request),
                cache: false,
                async: true
            });
        };
        this.GetValueTypeFunctions = (request) => {
            return jquery__WEBPACK_IMPORTED_MODULE_0___default().ajax({
                type: 'POST',
                url: `${this.GrafanaAPIPath}/GetValueTypeFunctions`,
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                data: JSON.stringify(request),
                cache: false,
                async: true
            });
        };
        this.GetValueTypeTables = (request) => {
            return jquery__WEBPACK_IMPORTED_MODULE_0___default().ajax({
                type: 'POST',
                url: `${this.GrafanaAPIPath}/GetValueTypeTables `,
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                data: JSON.stringify(request),
                cache: false,
                async: true
            });
        };
        this.GetValueTypes = () => {
            return jquery__WEBPACK_IMPORTED_MODULE_0___default().ajax({
                type: 'POST',
                url: `${this.GrafanaAPIPath}/GetValueTypes `,
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                cache: false,
                async: true
            });
        };
        this.Query = (request) => {
            return jquery__WEBPACK_IMPORTED_MODULE_0___default().ajax({
                type: 'POST',
                url: `${this.GrafanaAPIPath}/Query `,
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                cache: false,
                async: true,
                data: JSON.stringify(request)
            });
        };
        this.GrafanaAPIPath = GrafanaAPIPath;
    }
}


/***/ }),

/***/ "../../../../Common/TSX/global.ts":
/*!****************************************!*\
  !*** ../../../../Common/TSX/global.ts ***!
  \****************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   PhasorProtocol: () => (/* binding */ PhasorProtocol),
/* harmony export */   PhasorProtocolDataType: () => (/* binding */ PhasorProtocolDataType),
/* harmony export */   units: () => (/* binding */ units)
/* harmony export */ });
const units = ['ms', 's', 'm', 'h', 'd', 'w', 'M', 'y'];
//Enumns
var PhasorProtocolDataType;
(function (PhasorProtocolDataType) {
    PhasorProtocolDataType[PhasorProtocolDataType["String"] = 0] = "String";
    PhasorProtocolDataType[PhasorProtocolDataType["Int16"] = 1] = "Int16";
    PhasorProtocolDataType[PhasorProtocolDataType["UInt16"] = 2] = "UInt16";
    PhasorProtocolDataType[PhasorProtocolDataType["Int32"] = 3] = "Int32";
    PhasorProtocolDataType[PhasorProtocolDataType["UInt32"] = 4] = "UInt32";
    PhasorProtocolDataType[PhasorProtocolDataType["Int64"] = 5] = "Int64";
    PhasorProtocolDataType[PhasorProtocolDataType["UInt64"] = 6] = "UInt64";
    PhasorProtocolDataType[PhasorProtocolDataType["Single"] = 7] = "Single";
    PhasorProtocolDataType[PhasorProtocolDataType["Double"] = 8] = "Double";
    PhasorProtocolDataType[PhasorProtocolDataType["DateTime"] = 9] = "DateTime";
    PhasorProtocolDataType[PhasorProtocolDataType["Boolean"] = 10] = "Boolean";
    PhasorProtocolDataType[PhasorProtocolDataType["Enum"] = 11] = "Enum";
})(PhasorProtocolDataType || (PhasorProtocolDataType = {}));
var PhasorProtocol;
(function (PhasorProtocol) {
    PhasorProtocol[PhasorProtocol["IEEEC37_118V2"] = 0] = "IEEEC37_118V2";
    PhasorProtocol[PhasorProtocol["IEEEC37_118V1"] = 1] = "IEEEC37_118V1";
    PhasorProtocol[PhasorProtocol["IEEEC37_118D6"] = 2] = "IEEEC37_118D6";
    PhasorProtocol[PhasorProtocol["IEEE1344"] = 3] = "IEEE1344";
    PhasorProtocol[PhasorProtocol["BPAPDCstream"] = 4] = "BPAPDCstream";
    PhasorProtocol[PhasorProtocol["FNET"] = 5] = "FNET";
    PhasorProtocol[PhasorProtocol["SelFastMessage"] = 6] = "SelFastMessage";
    PhasorProtocol[PhasorProtocol["Macrodyne"] = 7] = "Macrodyne";
    PhasorProtocol[PhasorProtocol["IEC61850_90_5"] = 8] = "IEC61850_90_5";
})(PhasorProtocol || (PhasorProtocol = {}));


/***/ })

}]);