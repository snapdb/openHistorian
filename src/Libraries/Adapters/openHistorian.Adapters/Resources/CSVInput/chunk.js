(self["webpackChunkcsvinputadapter"] = self["webpackChunkcsvinputadapter"] || []).push([["TSX_Input_openHistorian_Adapters_CsvInputAdapter_openHistorian_Adapters_CsvInputAdapter_tsx"],{

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/AggregatingCircles.js":
/*!**************************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/AggregatingCircles.js ***!
  \**************************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

// ******************************************************************************************************
//  AggregatingCircles.tsx - Gbtc
//
//  Copyright © 2023, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  03/02/2023 - C Lackner
//       Generated original version of source code.
//
// ******************************************************************************************************
var __assign = (this && this.__assign) || function () {
    __assign = Object.assign || function(t) {
        for (var s, i = 1, n = arguments.length; i < n; i++) {
            s = arguments[i];
            for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p))
                t[p] = s[p];
        }
        return t;
    };
    return __assign.apply(this, arguments);
};
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
var Circle_1 = __webpack_require__(/*! ./Circle */ "./node_modules/@gpa-gemstone/react-graph/lib/Circle.js");
var AggregatingCircles = function (props) {
    /*
      Circle that will aggregate into larger circles
    */
    var context = React.useContext(GraphContext_1.GraphContext);
    var _a = __read(React.useState([]), 2), aggregate = _a[0], setAggregate = _a[1];
    // Optional prop to prevent aggregating into groups
    var useSingleAggregation = props.useSingleAggregation === undefined ? false : props.useSingleAggregation;
    // Re-calculate aggregation when data or context changes
    React.useEffect(function () {
        setAggregate(cluster(props.data));
    }, [props.data, context.UpdateFlag]);
    // Cluster circles based on aggregation criteria
    function cluster(circles) {
        var singleCircles = circles.map(function (c) { return (__assign({}, c)); });
        var clusters = [];
        // Define transformation functions using the context
        var fctn = {
            YInverseTransformation: context.YInverseTransformation,
            XInverseTransformation: context.XInverseTransformation,
            YTransformation: context.YTransformation,
            XTransformation: context.XTransformation
        };
        var _loop_1 = function (i) {
            var c1 = clusters.findIndex(function (c) { return c.Indices.includes(i); });
            var _loop_3 = function (j) {
                var _a;
                if (!props.canAggregate(singleCircles[i], singleCircles[j], fctn))
                    return "continue";
                var c2 = clusters.findIndex(function (c) { return c.Indices.includes(j); });
                // Handle various scenarios for merging and creating new clusters
                if (c1 < 0 && c2 < 0) {
                    clusters.push({ Indices: [i, j], Aggregate: null });
                    c1 = clusters.length - 1;
                    return "continue";
                }
                if (c1 === c2)
                    return "continue";
                if (c1 >= 0 && c2 < 0) {
                    clusters[c1].Indices.push(j);
                }
                if (c1 < 0 && c2 >= 0) {
                    clusters[c2].Indices.push(i);
                    c1 = clusters.length - 1;
                    return "continue";
                }
                if (c1 >= 0 && c2 >= 0) {
                    (_a = clusters[c1].Indices).push.apply(_a, __spreadArray([], __read(clusters[c2].Indices), false));
                    clusters.splice(c2, 1);
                    c1 = clusters.findIndex(function (c) { return c.Indices.includes(i); });
                }
            };
            for (var j = i + 1; j < singleCircles.length; j = j + 1) {
                _loop_3(j);
            }
        };
        // Cluster start to cluster based on single circles
        for (var i = 0; i < singleCircles.length; i = i + 1) {
            _loop_1(i);
        }
        var NClusters = clusters.length;
        var NClustered = clusters.reduce(function (s, c) { return s + c.Indices.length; }, 0);
        clusters.forEach(function (c) {
            c.Aggregate = props.onAggregation(singleCircles.filter(function (x, i) { return c.Indices.includes(i); }), fctn);
        });
        // If not using single aggregation mode, perform further aggregation
        if (!useSingleAggregation && NClusters > 0) {
            var _loop_2 = function () {
                NClusters = clusters.length;
                NClustered = clusters.reduce(function (s, c) { return s + c.Indices.length; }, 0);
                // clusters with index in 0 are replaced with clusters in index 1 (always remove i)
                var clusterReplacements = [];
                var _loop_4 = function (i) {
                    var replacementCluster = i;
                    var _loop_5 = function (j) {
                        var _b;
                        if (!props.canAggregate(clusters[i].Aggregate, clusters[j].Aggregate, fctn))
                            return "continue";
                        clusterReplacements.push(i);
                        (_b = clusters[j].Indices).push.apply(_b, __spreadArray([], __read(clusters[i].Indices), false));
                        clusters[j].Aggregate = props.onAggregation(singleCircles.filter(function (x, l) { return clusters[j].Indices.includes(l); }), fctn);
                        replacementCluster = j;
                        return "break";
                    };
                    for (var j = i + 1; j < clusters.length; j = j + 1) {
                        var state_1 = _loop_5(j);
                        if (state_1 === "break")
                            break;
                    }
                    var _loop_6 = function (j) {
                        if (clusters.findIndex(function (cl) { return cl.Indices.includes(j); }) > -1)
                            return "continue";
                        if (!props.canAggregate(clusters[replacementCluster].Aggregate, singleCircles[j], fctn))
                            return "continue";
                        clusters[replacementCluster].Indices.push(j);
                        clusters[replacementCluster].Aggregate = props.onAggregation(singleCircles.filter(function (x, l) { return clusters[replacementCluster].Indices.includes(l); }), fctn);
                    };
                    for (var j = 0; j < singleCircles.length; j = j + 1) {
                        _loop_6(j);
                    }
                };
                for (var i = 0; i < clusters.length; i = i + 1) {
                    _loop_4(i);
                }
                clusters = clusters.filter(function (c, l) { return !clusterReplacements.includes(l); });
            };
            do {
                _loop_2();
            } while (NClusters !== clusters.length || NClustered !== clusters.reduce(function (s, c) { return s + c.Indices.length; }, 0));
        }
        // Return a combination of single circles not in any cluster and the aggregated circles
        return __spreadArray(__spreadArray([], __read(singleCircles.filter(function (c, i) { return clusters.findIndex(function (cl) { return cl.Indices.includes(i); }) === -1; })), false), __read(clusters.map(function (c) { return c.Aggregate; })), false);
    }
    return (React.createElement("g", null, aggregate.map(function (c, i) { return React.createElement(Circle_1.ContextlessCircle, { key: i.toString() + (c.text === undefined ? '' : c.text), circleProps: c, context: context }); })));
};
exports["default"] = AggregatingCircles;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiQWdncmVnYXRpbmdDaXJjbGVzLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vc3JjL0FnZ3JlZ2F0aW5nQ2lyY2xlcy50c3giXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IjtBQUFBLHlHQUF5RztBQUN6RyxpQ0FBaUM7QUFDakMsRUFBRTtBQUNGLHFFQUFxRTtBQUNyRSxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4RyxzR0FBc0c7QUFDdEcsd0ZBQXdGO0FBQ3hGLEVBQUU7QUFDRiwwQ0FBMEM7QUFDMUMsRUFBRTtBQUNGLHdHQUF3RztBQUN4Ryx3R0FBd0c7QUFDeEcsNEVBQTRFO0FBQzVFLEVBQUU7QUFDRiw4QkFBOEI7QUFDOUIsd0dBQXdHO0FBQ3hHLDBCQUEwQjtBQUMxQixtREFBbUQ7QUFDbkQsRUFBRTtBQUNGLHlHQUF5Rzs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7QUFHekcsNkJBQStCO0FBQy9CLCtDQUE4RDtBQUM5RCxtQ0FBb0U7QUFzQnBFLElBQU0sa0JBQWtCLEdBQUcsVUFBQyxLQUFhO0lBQ3ZDOztNQUVFO0lBRUYsSUFBTSxPQUFPLEdBQUcsS0FBSyxDQUFDLFVBQVUsQ0FBQywyQkFBWSxDQUFDLENBQUE7SUFDeEMsSUFBQSxLQUFBLE9BQTRCLEtBQUssQ0FBQyxRQUFRLENBQWlCLEVBQUUsQ0FBQyxJQUFBLEVBQTdELFNBQVMsUUFBQSxFQUFFLFlBQVksUUFBc0MsQ0FBQTtJQUVwRSxtREFBbUQ7SUFDakQsSUFBTSxvQkFBb0IsR0FBRyxLQUFLLENBQUMsb0JBQW9CLEtBQUssU0FBUyxDQUFDLENBQUMsQ0FBQyxLQUFLLENBQUMsQ0FBQyxDQUFDLEtBQUssQ0FBQyxvQkFBb0IsQ0FBQztJQUU3Ryx3REFBd0Q7SUFDeEQsS0FBSyxDQUFDLFNBQVMsQ0FBQztRQUNaLFlBQVksQ0FBQyxPQUFPLENBQUMsS0FBSyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUM7SUFDdEMsQ0FBQyxFQUFFLENB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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/Button.js":
/*!**************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/Button.js ***!
  \**************************************************************/
/***/ ((__unused_webpack_module, exports, __webpack_require__) => {

"use strict";

// ******************************************************************************************************
//  Button.tsx - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  04/20/2022 - G Santos
//       Generated original version of source code.
//
// ******************************************************************************************************
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var Button = function (props) {
    /*
      Button that can be pressed.
    */
    return (React.createElement(React.Fragment, null, props.children));
};
exports["default"] = Button;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiQnV0dG9uLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vc3JjL0J1dHRvbi50c3giXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IjtBQUFBLHlHQUF5RztBQUN6RyxxQkFBcUI7QUFDckIsRUFBRTtBQUNGLHFFQUFxRTtBQUNyRSxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4RyxzR0FBc0c7QUFDdEcsd0ZBQXdGO0FBQ3hGLEVBQUU7QUFDRiwwQ0FBMEM7QUFDMUMsRUFBRTtBQUNGLHdHQUF3RztBQUN4Ryx3R0FBd0c7QUFDeEcsNEVBQTRFO0FBQzVFLEVBQUU7QUFDRiw4QkFBOEI7QUFDOUIsd0dBQXdHO0FBQ3hHLHlCQUF5QjtBQUN6QixtREFBbUQ7QUFDbkQsRUFBRTtBQUNGLHlHQUF5Rzs7QUFFekcsNkJBQStCO0FBTy9CLElBQU0sTUFBTSxHQUFvQyxVQUFDLEtBQUs7SUFDcEQ7O01BRUU7SUFFRCxPQUFPLENBQ04sMENBQ0csS0FBSyxDQUFDLFFBQVEsQ0FDZCxDQUNILENBQUM7QUFDTCxDQUFDLENBQUE7QUFFRCxrQkFBZSxNQUFNLENBQUMifQ==

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/Circle.js":
/*!**************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/Circle.js ***!
  \**************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

// ******************************************************************************************************
//  Circle.tsx - Gbtc
//
//  Copyright © 2023, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  03/02/2023 - C Lackner
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.ContextlessCircle = ContextlessCircle;
var helper_functions_1 = __webpack_require__(/*! @gpa-gemstone/helper-functions */ "./node_modules/@gpa-gemstone/helper-functions/lib/index.js");
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
function ContextlessCircle(props) {
    /*
      Circle with basic styling
    */
    var _a = __read(React.useState(""), 2), guid = _a[0], setGuid = _a[1];
    var _b = __read(React.useState(1), 2), textSize = _b[0], setTextSize = _b[1];
    // Update data series information in the graph context based on circle properties
    React.useEffect(function () {
        if (guid === "")
            return;
        props.context.UpdateData(guid, {
            axis: props.circleProps.axis,
            legend: undefined,
            getMax: function (t) { return (t[0] < props.circleProps.data[0] && t[1] > props.circleProps.data[0] ? props.circleProps.data[1] : undefined); },
            getMin: function (t) { return (t[0] < props.circleProps.data[0] && t[1] > props.circleProps.data[0] ? props.circleProps.data[1] : undefined); },
        });
    }, [props.circleProps]);
    // Add a new data series on component mount / removing on unmount
    React.useEffect(function () {
        var id = props.context.AddData({
            axis: props.circleProps.axis,
            legend: undefined,
            getMax: function (t) { return (t[0] < props.circleProps.data[0] && t[1] > props.circleProps.data[0] ? props.circleProps.data[1] : undefined); },
            getMin: function (t) { return (t[0] < props.circleProps.data[0] && t[1] > props.circleProps.data[0] ? props.circleProps.data[1] : undefined); },
        });
        setGuid(id);
        return function () { props.context.RemoveData(id); };
    }, []);
    // Adjust text size within the circle to ensure it fits
    React.useEffect(function () {
        if (props.circleProps.text === undefined)
            return;
        var tSize = 5;
        var dX = (0, helper_functions_1.GetTextWidth)("Segoe UI", tSize + "em", props.circleProps.text);
        var dY = (0, helper_functions_1.GetTextHeight)("Segoe UI", tSize + "em", props.circleProps.text);
        while ((dX > 2 * props.circleProps.radius || dY > 2 * props.circleProps.radius) && tSize > 0.05) {
            tSize = tSize - 0.01;
            dX = (0, helper_functions_1.GetTextWidth)("Segoe UI", tSize + "em", props.circleProps.text);
            dY = (0, helper_functions_1.GetTextHeight)("Segoe UI", tSize + "em", props.circleProps.text);
        }
        setTextSize(tSize);
    }, [props.circleProps.text, props.circleProps.radius]);
    // Set up a click handler if provided in props
    React.useEffect(function () {
        if (guid === "" || props.circleProps.onClick === undefined)
            return;
        props.context.UpdateSelect(guid, {
            onClick: onClick
        });
    }, [props.circleProps.onClick, props.context.UpdateFlag]);
    // Handle click events on the circle
    function onClick(x, y) {
        if (props.circleProps.onClick === undefined)
            return;
        // Calculate positions and determine if the click was within the circle bounds
        var axis = GraphContext_1.AxisMap.get(props.circleProps.axis);
        var xP = props.context.XTransformation(x);
        var yP = props.context.YTransformation(y, axis);
        var xC = props.context.XTransformation(props.circleProps.data[0]);
        var yC = props.context.YTransformation(props.circleProps.data[1], axis);
        if (xP <= xC + props.circleProps.radius && xP >= xC - props.circleProps.radius &&
            yP <= yC + props.circleProps.radius && yP >= yC - props.circleProps.radius)
            props.circleProps.onClick({
                setYDomain: props.context.SetYDomain,
                setTDomain: props.context.SetXDomain
            });
    }
    // Render null if coordinates are not valid, otherwise render the circle / text
    if (!isFinite(props.context.XTransformation(props.circleProps.data[0])) || !isFinite(props.context.YTransformation(props.circleProps.data[1], GraphContext_1.AxisMap.get(props.circleProps.axis))))
        return null;
    return (React.createElement("g", null,
        React.createElement("circle", { r: props.circleProps.radius, cx: props.context.XTransformation(props.circleProps.data[0]), cy: props.context.YTransformation(props.circleProps.data[1], GraphContext_1.AxisMap.get(props.circleProps.axis)), fill: props.circleProps.color, opacity: props.circleProps.opacity, stroke: props.circleProps.borderColor, strokeWidth: props.circleProps.borderThickness }),
        props.circleProps.text !== undefined ? React.createElement("text", { fill: 'black', style: { fontSize: textSize + 'em', textAnchor: 'middle', dominantBaseline: 'middle' }, y: props.context.YTransformation(props.circleProps.data[1], GraphContext_1.AxisMap.get(props.circleProps.axis)), x: props.context.XTransformation(props.circleProps.data[0]) }, props.circleProps.text) : null));
}
// Higher-order component that uses GraphContext to pass down context to the ContextlessCircle
var Circle = function (props) {
    var context = React.useContext(GraphContext_1.GraphContext);
    return React.createElement(ContextlessCircle, { circleProps: props, context: context });
};
exports["default"] = Circle;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiQ2lyY2xlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vc3JjL0NpcmNsZS50c3giXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IjtBQUFBLHlHQUF5RztBQUN6RyxxQkFBcUI7QUFDckIsRUFBRTtBQUNGLHFFQUFxRTtBQUNyRSxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4RyxzR0FBc0c7QUFDdEcsd0ZBQXdGO0FBQ3hGLEVBQUU7QUFDRiwwQ0FBMEM7QUFDMUMsRUFBRTtBQUNGLHdHQUF3RztBQUN4Ryx3R0FBd0c7QUFDeEcsNEVBQTRFO0FBQzVFLEVBQUU7QUFDRiw4QkFBOEI7QUFDOUIsd0dBQXdHO0FBQ3hHLDBCQUEwQjtBQUMxQixtREFBbUQ7QUFDbkQsRUFBRTtBQUNGLHlHQUF5Rzs7Ozs7Ozs7Ozs7Ozs7Ozs7O0FBd0J6Ryw4Q0EwR0M7QUEvSEQsbUVBQTZFO0FBQzdFLDZCQUErQjtBQUMvQiwrQ0FBOEg7QUFtQjlILFNBQWdCLGlCQUFpQixDQUFDLEtBQXdCO0lBQ3hEOztNQUVFO0lBQ0ksSUFBQSxLQUFBLE9BQWtCLEtBQUssQ0FBQyxRQUFRLENBQVMsRUFBRSxDQUFDLElBQUEsRUFBM0MsSUFBSSxRQUFBLEVBQUUsT0FBTyxRQUE4QixDQUFDO0lBQzdDLElBQUEsS0FBQSxPQUEwQixLQUFLLENBQUMsUUFBUSxDQUFTLENBQUMsQ0FBQyxJQUFBLEVBQWxELFFBQVEsUUFBQSxFQUFFLFdBQVcsUUFBNkIsQ0FBQztJQUUxRCxpRkFBaUY7SUFDakYsS0FBSyxDQUFDLFNBQVMsQ0FBQztRQUNkLElBQUksSUFBSSxLQUFLLEVBQUU7WUFDWCxPQUFPO1FBRVgsS0FBSyxDQUFDLE9BQU8sQ0FBQyxVQUFVLENBQUMsSUFBSSxFQUFFO1lBQzNCLElBQUksRUFBRSxLQUFLLENBQUMsV0FBVyxDQUFDLElBQUk7WUFDNUIsTUFBTSxFQUFFLFNBQVM7WUFDakIsTUFBTSxFQUFFLFVBQUMsQ0FBQyxJQUFLLE9BQUEsQ0FBQyxDQUFDLENB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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js":
/*!********************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js ***!
  \********************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.ContextWrapper = exports.AxisMap = exports.LineMap = exports.GraphContext = void 0;
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
exports.GraphContext = React.createContext({
    XDomain: [0, 0],
    XHover: NaN,
    XHoverSnap: NaN,
    YHover: [NaN, NaN],
    YHoverSnap: [NaN, NaN],
    YDomain: [[0, 0]],
    CurrentMode: 'select',
    Data: React.createRef(),
    DataGuid: "",
    XApplyPixelOffset: function (_) { return _; },
    YApplyPixelOffset: function (_) { return _; },
    XTransformation: function (_) { return 0; },
    YTransformation: function (_, __) { return 0; },
    XInverseTransformation: function (_) { return 0; },
    YInverseTransformation: function (_, __) { return 0; },
    AddData: (function (_) { return ""; }),
    RemoveData: function (_) { return undefined; },
    UpdateData: function (_) { return undefined; },
    SetLegend: function (_) { return undefined; },
    RegisterSelect: function (_) { return ""; },
    RemoveSelect: function (_) { return undefined; },
    UpdateSelect: function (_) { return undefined; },
    SetXDomain: function (_) { return undefined; },
    SetYDomain: function (_) { return undefined; },
    UpdateFlag: 0
});
exports.LineMap = new Map([
    ['-', 'none'],
    ['solid', 'none'],
    [':', '10,5'],
    ['short-dash', '10,5'],
    ['dash', '20,5'],
    ['long-dash', '30,5']
]);
var AxisMapClass = /** @class */ (function () {
    function AxisMapClass(iterable, undefinedOverride) {
        var _this = this;
        this.get = function (key) { var _a; return ((_a = _this.mapBase.get(key)) !== null && _a !== void 0 ? _a : _this.undefinedOverride); };
        this.values = function () { return (_this.mapBase.values()); };
        this.keys = function () { return (_this.mapBase.keys()); };
        this.mapBase = new Map(iterable);
        this.undefinedOverride = undefinedOverride;
        // Note: if we ever allow mapBase to be mutated, the mutate methods should assign this
        this.size = this.mapBase.size;
    }
    return AxisMapClass;
}());
// Giving this undefined (such as when an axis for a component is not specfied), will return 0, same as making a default of 'left'
exports.AxisMap = new AxisMapClass([
    ['left', 0],
    ['right', 1]
], 0);
var ContextWrapper = function (props) {
    var context = React.useMemo(GetContext, [
        props.XDomain,
        props.MousePosition,
        props.MousePositionSnap,
        props.YDomain,
        props.CurrentMode,
        props.MouseIn,
        props.UpdateFlag,
        props.DataGuid,
        props.XApplyPixelOffset,
        props.YApplyPixelOffset,
        props.XTransform,
        props.XInvTransform,
        props.YInvTransform,
        props.YTransform,
        props.SetXDomain,
        props.SetYDomain,
        props.AddData,
        props.RemoveData,
        props.UpdateData,
        props.SetLegend,
        props.RegisterSelect,
        props.RemoveSelect,
        props.UpdateSelect
    ]);
    function GetContext() {
        return {
            XDomain: props.XDomain,
            XHover: props.MouseIn ? props.XInvTransform(props.MousePosition[0]) : NaN,
            YHover: props.MouseIn ? __spreadArray([], __read(exports.AxisMap.values()), false).map(function (axis) { return props.YInvTransform(props.MousePosition[1], axis); }) : Array(exports.AxisMap.size).fill(NaN),
            XHoverSnap: props.MouseIn ? props.XInvTransform(props.MousePositionSnap[0]) : NaN,
            YHoverSnap: props.MouseIn ? __spreadArray([], __read(exports.AxisMap.values()), false).map(function (axis) { return props.YInvTransform(props.MousePositionSnap[1], axis); }) : Array(exports.AxisMap.size).fill(NaN),
            YDomain: props.YDomain,
            CurrentMode: props.CurrentMode,
            Data: props.Data,
            DataGuid: props.DataGuid,
            XApplyPixelOffset: props.XApplyPixelOffset,
            YApplyPixelOffset: props.YApplyPixelOffset,
            XTransformation: props.XTransform,
            YTransformation: props.YTransform,
            XInverseTransformation: props.XInvTransform,
            YInverseTransformation: props.YInvTransform,
            AddData: props.AddData,
            RemoveData: props.RemoveData,
            UpdateData: props.UpdateData,
            SetLegend: props.SetLegend,
            RegisterSelect: props.RegisterSelect,
            RemoveSelect: props.RemoveSelect,
            UpdateSelect: props.UpdateSelect,
            UpdateFlag: props.UpdateFlag,
            SetXDomain: props.SetXDomain,
            SetYDomain: props.SetYDomain
        };
    }
    return React.createElement(exports.GraphContext.Provider, { value: context }, props.children);
};
exports.ContextWrapper = ContextWrapper;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiR3JhcGhDb250ZXh0LmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vc3JjL0dyYXBoQ29udGV4dC50c3giXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6Ijs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7OztBQXVCQSw2QkFBK0I7QUF5QmxCLFFBQUEsWUFBWSxHQUFHLEtBQUssQ0FBQyxhQUFhLENBQUM7SUFDOUMsT0FBTyxFQUFFLENBQUMsQ0FBQyxFQUFFLENBQUMsQ0FBQztJQUNmLE1BQU0sRUFBRSxHQUFHO0lBQ1gsVUFBVSxFQUFFLEdBQUc7SUFFZixNQUFNLEVBQUUsQ0FBQyxHQUFHLEVBQUUsR0FBRyxDQUFDO0lBQ2xCLFVBQVUsRUFBRSxDQUFDLEdBQUcsRUFBRSxHQUFHLENBQUM7SUFDdEIsT0FBTyxFQUFFLENBQUMsQ0FBQyxDQUFDLEVBQUUsQ0FBQyxDQUFDLENBQUM7SUFDakIsV0FBVyxFQUFFLFFBQVE7SUFHckIsSUFBSSxFQUFFLEtBQUssQ0FBQyxTQUFTLEVBQUU7SUFDdkIsUUFBUSxFQUFFLEVBQUU7SUFDWixpQkFBaUIsRUFBRSxVQUFDLENBQVMsSUFBSyxPQUFBLENBQUMsRUFBRCxDQUFDO0lBQ25DLGlCQUFpQixFQUFFLFVBQUMsQ0FBUyxJQUFLLE9BQUEsQ0FBQyxFQUFELENBQUM7SUFDbkMsZUFBZSxFQUFFLFVBQUMsQ0FBUyxJQUFLLE9BQUEsQ0FBQyxFQUFELENBQUM7SUFDakMsZUFBZSxFQUFFLFVBQUMsQ0FBUyxFQUFFLEVBQXlCLElBQUssT0FBQSxDQUFDLEVBQUQsQ0FBQztJQUM1RCxzQkFBc0IsRUFBRSxVQUFDLENBQVMsSUFBSyxPQUFBLENBQUMsRUFBRCxDQUFDO0lBQ3hDLHNCQUFzQixFQUFFLFVBQUMsQ0FBUyxFQUFFLEVBQXlCLElBQUssT0FBQSxDQUFDLEVBQUQsQ0FBQztJQUNuRSxPQUFPLEVBQUUsQ0FBQyxVQUFDLENBQWMsSUFBSyxPQUFBLEVBQUUsRUFBRixDQUFFLENBQUM7SUFDakMsVUFBVSxFQUFF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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/HeatLegend.js":
/*!******************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/HeatLegend.js ***!
  \******************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

// ******************************************************************************************************
//  HeatLegend.tsx - Gbtc
//
//  Copyright © 2023, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  11/21/2023 - G. Santos
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var LegendContext_1 = __webpack_require__(/*! ./LegendContext */ "./node_modules/@gpa-gemstone/react-graph/lib/LegendContext.js");
var helper_functions_1 = __webpack_require__(/*! @gpa-gemstone/helper-functions */ "./node_modules/@gpa-gemstone/helper-functions/lib/index.js");
var SvgStyle = {
    fill: 'none',
    userSelect: 'none',
    WebkitTouchCallout: 'none',
    WebkitUserSelect: 'none',
    KhtmlUserSelect: 'none',
    MozUserSelect: 'none',
    msUserSelect: 'none',
    pointerEvents: 'none',
};
var TextStyle = {
    fontSize: '1em',
    textAnchor: 'middle',
    dominantBaseline: 'hanging',
    transition: 'x 0.5s, y 0.5s'
};
function HeatLegend(props) {
    var _a = __read(React.useState(100), 2), wLegend = _a[0], setWLegend = _a[1];
    var _b = __read(React.useState(100), 2), hLegend = _b[0], setHLegend = _b[1];
    var _c = __read(React.useState(1), 2), nDigits = _c[0], setNdigits = _c[1];
    var _d = __read(React.useState(''), 2), guid = _d[0], setGuid = _d[1];
    var context = React.useContext(LegendContext_1.LegendContext);
    // Effect to update the legend's width and height based on the container's dimensions
    React.useEffect(function () { return setWLegend(props.size === 'sm' ? context.SmWidth : context.LgWidth); }, [context.LgWidth, context.SmWidth, props.size]);
    React.useEffect(function () { return setHLegend(props.size === 'sm' ? context.SmHeight : context.LgHeight); }, [context.SmHeight, context.LgHeight, props.size]);
    // Determine the number of decimal digits to display based on the value range
    React.useEffect(function () {
        var delta = props.maxValue - props.minValue;
        if (delta === 0)
            delta = Math.abs(props.minValue);
        if (delta >= 15)
            setNdigits(0);
        if (delta < 15 && delta >= 1.5)
            setNdigits(1);
        if (delta < 1.5 && delta >= 0.15)
            setNdigits(2);
        if (delta < 0.15)
            setNdigits(3);
        if (delta < 0.015)
            setNdigits(4);
        if (delta < 0.0015)
            setNdigits(5);
        if (delta === 0)
            setNdigits(2);
    }, [props.maxValue, props.minValue]);
    // Generate a unique ID for the gradient and request space for the legend
    React.useEffect(function () {
        var id = (0, helper_functions_1.CreateGuid)();
        setGuid(id);
        context.RequestLegendWidth(50, id);
        return function () { context.RequestLegendWidth(-1, id); };
    }, []);
    return (React.createElement("div", { style: { height: hLegend, width: wLegend } },
        React.createElement("div", { style: { width: '100%', display: 'flex', alignItems: 'center', marginRight: '5px', height: '100%' } },
            React.createElement("svg", { style: SvgStyle, viewBox: "0 0 ".concat(wLegend, " ").concat(hLegend) },
                React.createElement("linearGradient", { id: guid, x1: "0", x2: "".concat(wLegend < hLegend ? 0 : 1), y1: "0", y2: "".concat(wLegend < hLegend ? 1 : 0) },
                    React.createElement("stop", { offset: "5%", stopColor: props.minColor }),
                    React.createElement("stop", { offset: "95%", stopColor: props.maxColor })),
                React.createElement("path", { stroke: 'black', fill: "url(#".concat(guid, ")"), style: { strokeWidth: 1, transition: 'd 0.5s' }, d: wLegend < hLegend ?
                        "M ".concat(0.05 * wLegend, " ").concat(0.1 * hLegend, " H ").concat(0.5 * wLegend, " V ").concat(0.9 * hLegend, " H ").concat(0.05 * wLegend, " V ").concat(0.1 * hLegend) :
                        "M ".concat(0.1 * wLegend, " ").concat(0.05 * hLegend, " H ").concat(0.9 * wLegend, " V ").concat(0.5 * hLegend, " H ").concat(0.1 * wLegend, " V ").concat(0.05 * hLegend) }),
                React.createElement("text", { fill: 'black', style: TextStyle, x: wLegend * (wLegend < hLegend ? 0.5 : 0.1), y: hLegend * (wLegend < hLegend ? 0.1 : 0.5), transform: "rotate(".concat(wLegend < hLegend ? 270 : 0, ",").concat(wLegend * (wLegend < hLegend ? 0.5 : 0.1), ",").concat(hLegend * (wLegend < hLegend ? 0.1 : 0.5), ")") }, "".concat(props.minValue.toFixed(nDigits)).concat(props.unitLabel !== undefined ? "".concat(props.unitLabel) : '')),
                React.createElement("text", { fill: 'black', style: TextStyle, x: wLegend * (wLegend < hLegend ? 0.5 : 0.9), y: hLegend * (wLegend < hLegend ? 0.9 : 0.5), transform: "rotate(".concat(wLegend < hLegend ? 270 : 0, ",").concat(wLegend * (wLegend < hLegend ? 0.5 : 0.9), ",").concat(hLegend * (wLegend < hLegend ? 0.9 : 0.5), ")") }, "".concat(props.maxValue.toFixed(nDigits)).concat(props.unitLabel !== undefined ? "".concat(props.unitLabel) : ''))))));
}
exports["default"] = HeatLegend;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSGVhdExlZ2VuZC5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uL3NyYy9IZWF0TGVnZW5kLnRzeCJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiO0FBQUEseUdBQXlHO0FBQ3pHLHlCQUF5QjtBQUN6QixFQUFFO0FBQ0YscUVBQXFFO0FBQ3JFLEVBQUU7QUFDRix3R0FBd0c7QUFDeEcsd0dBQXdHO0FBQ3hHLHNHQUFzRztBQUN0Ryx3RkFBd0Y7QUFDeEYsRUFBRTtBQUNGLDBDQUEwQztBQUMxQyxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4Ryw0RUFBNEU7QUFDNUUsRUFBRTtBQUNGLDhCQUE4QjtBQUM5Qix3R0FBd0c7QUFDeEcsMEJBQTBCO0FBQzFCLG1EQUFtRDtBQUNuRCxFQUFFO0FBQ0YseUdBQXlHOzs7Ozs7Ozs7Ozs7Ozs7Ozs7QUFHekcsNkJBQStCO0FBQy9CLGlEQUFzRTtBQUN0RSxtRUFBNEQ7QUFXNUQsSUFBTSxRQUFRLEdBQXdCO0lBQ3BDLElBQUksRUFBRSxNQUFNO0lBQ1osVUFBVSxFQUFFLE1BQU07SUFDbEIsa0JBQWtCLEVBQUUsTUFBTTtJQUMxQixnQkFBZ0IsRUFBRSxNQUFNO0lBQ3hCLGVBQWUsRUFBRSxNQUFNO0lBQ3ZCLGFBQWEsRUFBRSxNQUFNO0lBQ3JCLFlBQVksRUFBRSxNQUFNO0lBQ3BCLGFBQWEsRUFBRSxNQUFNO0NBQ3RCLENBQUM7QUFFRixJQUFNLFNBQVMsR0FBd0I7SUFDckMsUUFBUSxFQUFFLEtBQUs7SUFDZixVQUFVLEVBQUUsUUFBUTtJQUNwQixnQkFBZ0IsRUFBRSxTQUFTO0lBQzNCLFVBQVUsRUFBRSxnQkFBZ0I7Q0FDN0IsQ0FBQztBQUVGLFNBQVMsVUFBVSxDQUFDLEtBQWE7SUFDekIsSUFBQSxLQUFBLE9BQXdCLEtBQUssQ0FBQyxRQUFRLENBQVMsR0FBRyxDQUFDLElBQUEsRUFBbEQsT0FBTyxRQUFBLEVBQUUsVUFBVSxRQUErQixDQUFDO0lBQ3BELElBQUEsS0FBQSxPQUF3QixL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LEdBQUcsR0FBQyxPQUFPLGNBQUksSUFBSSxHQUFDLE9BQU8sZ0JBQU0sR0FBRyxHQUFDLE9BQU8sZ0JBQU0sR0FBRyxHQUFDLE9BQU8sZ0JBQU0sR0FBRyxHQUFDLE9BQU8sZ0JBQU0sSUFBSSxHQUFDLE9BQU8sQ0FBRSxHQUMzRztnQkFHRiw4QkFBTSxJQUFJLEVBQUUsT0FBTyxFQUFFLEtBQUssRUFBRSxTQUFTLEVBQUUsQ0FBQyxFQUFFLE9BQU8sR0FBQyxDQUFDLE9BQU8sR0FBRyxPQUFPLENBQUMsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxDQUFDLENBQUMsR0FBRyxDQUFDLEVBQUUsQ0FBQyxFQUFFLE9BQU8sR0FBQyxDQUFDLE9BQU8sR0FBRyxPQUFPLENBQUMsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxDQUFDLENBQUMsR0FBRyxDQUFDLEVBQzNILFNBQVMsRUFBRSxpQkFBVSxPQUFPLEdBQUcsT0FBTyxDQUFDLENBQUMsQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLENBQUMsY0FBSSxPQUFPLEdBQUMsQ0FBQyxPQUFPLEdBQUcsT0FBTyxDQUFDLENBQUMsQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxjQUFJLE9BQU8sR0FBQyxDQUFDLE9BQU8sR0FBRyxPQUFPLENBQUMsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxDQUFDLENBQUMsR0FBRyxDQUFDLE1BQUcsSUFDdEksVUFBRyxLQUFLLENBQUMsUUFBUSxDQUFDLE9BQU8sQ0FBQyxPQUFPLENBQUMsU0FBRyxLQUFLLENBQUMsU0FBUyxLQUFLLFNBQVMsQ0FBQyxDQUFDLENBQUMsVUFBRyxLQUFLLENBQUMsU0FBUyxDQUFFLENBQUMsQ0FBQyxDQUFDLEVBQUUsQ0FBRSxDQUM1RjtnQkFFVCw4QkFBTSxJQUFJLEVBQUUsT0FBTyxFQUFFLEtBQUssRUFBRSxTQUFTLEVBQUUsQ0FBQyxFQUFFLE9BQU8sR0FBQyxDQUFDLE9BQU8sR0FBRyxPQUFPLENBQUMsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxDQUFDLENBQUMsR0FBRyxDQUFDLEVBQUUsQ0FBQyxFQUFFLE9BQU8sR0FBQyxDQUFDLE9BQU8sR0FBRyxPQUFPLENBQUMsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxDQUFDLENBQUMsR0FBRyxDQUFDLEVBQzNILFNBQVMsRUFBRSxpQkFBVSxPQUFPLEdBQUcsT0FBTyxDQUFDLENBQUMsQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLENBQUMsY0FBSSxPQUFPLEdBQUMsQ0FBQyxPQUFPLEdBQUcsT0FBTyxDQUFDLENBQUMsQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxjQUFJLE9BQU8sR0FBQyxDQUFDLE9BQU8sR0FBRyxPQUFPLENBQUMsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxDQUFDLENBQUMsR0FBRyxDQUFDLE1BQUcsSUFDdEksVUFBRyxLQUFLLENBQUMsUUFBUSxDQUFDLE9BQU8sQ0FBQyxPQUFPLENBQUMsU0FBRyxLQUFLLENBQUMsU0FBUyxLQUFLLFNBQVMsQ0FBQyxDQUFDLENBQUMsVUFBRyxLQUFLLENBQUMsU0FBUyxDQUFFLENBQUMsQ0FBQyxDQUFDLEVBQUUsQ0FBRSxDQUM1RixDQUNILENBQ0YsQ0FDRixDQUNQLENBQUM7QUFDSixDQUFDO0FBRUQsa0JBQWUsVUFBVSxDQUFDIn0=

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/HeatMapChart.js":
/*!********************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/HeatMapChart.js ***!
  \********************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

// ******************************************************************************************************
//  HeatMapChart.tsx - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  09/05/2021 - G. Santos
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var helper_functions_1 = __webpack_require__(/*! @gpa-gemstone/helper-functions */ "./node_modules/@gpa-gemstone/helper-functions/lib/index.js");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
var PointNode_1 = __webpack_require__(/*! ./PointNode */ "./node_modules/@gpa-gemstone/react-graph/lib/PointNode.js");
var HeatLegend_1 = __webpack_require__(/*! ./HeatLegend */ "./node_modules/@gpa-gemstone/react-graph/lib/HeatLegend.js");
function HeatMapChart(props) {
    /*
        Single Line with ability to turn off and on.
    */
    var _a = __read(React.useState(""), 2), guid = _a[0], setGuid = _a[1];
    var _b = __read(React.useState(null), 2), data = _b[0], setData = _b[1];
    var _c = __read(React.useState(0), 2), barWidth = _c[0], setBarWidth = _c[1];
    var context = React.useContext(GraphContext_1.GraphContext);
    var allBarBottoms = React.useMemo(function () { return context.YTransformation(context.YDomain[GraphContext_1.AxisMap.get(props.axis)][0], GraphContext_1.AxisMap.get(props.axis)); }, [context.YTransformation, context.YDomain, props.axis]);
    var zLimits = React.useMemo(function () {
        if (data == null)
            return [0, 1];
        return data.GetLimits(context.XDomain[0], context.XDomain[1], 1);
    }, [data, context.XDomain]);
    function getAllBarOffset() {
        switch (props.barAlign) {
            case 'left':
                return 0;
            case 'center':
                return 0.5 * barWidth;
            case 'right':
                return barWidth;
        }
        return 0;
    }
    var allBarOffset = getAllBarOffset();
    React.useEffect(function () {
        if (data == null)
            return;
        if (props.sampleMs !== undefined) {
            setBarWidth(context.XTransformation(data.minT + props.sampleMs) - context.XTransformation(data.minT));
            return;
        }
        setBarWidth((context.XTransformation(data.maxT) - context.XTransformation(data.minT)) / data.GetFullData().length);
    }, [data, context.XTransformation, props.sampleMs]);
    var createLegend = React.useCallback(function () {
        return React.createElement(HeatLegend_1.default, { size: 'lg', unitLabel: props.legendUnit, enabled: true, minColor: (0, helper_functions_1.HsvToHex)(props.hue, props.saturation, 1), maxColor: (0, helper_functions_1.HsvToHex)(props.hue, props.saturation, 0), minValue: zLimits[0], maxValue: zLimits[1] });
    }, [props.legendUnit, zLimits, props.hue, props.saturation]);
    React.useEffect(function () {
        setData(new PointNode_1.PointNode(props.data));
    }, [props.data]);
    React.useEffect(function () {
        if (guid === "")
            return;
        context.UpdateData(guid, {
            axis: props.axis,
            legend: createLegend(),
            enabled: true,
            getMax: function (t) { return (data == null ? -Infinity : data.GetLimits(t[0], t[1], 0)[1]); },
            getMin: function (t) { return (data == null ? Infinity : data.GetLimits(t[0], t[1], 0)[0]); },
        });
    }, [props, data, createLegend]);
    React.useEffect(function () {
        var id = context.AddData({
            axis: props.axis,
            legend: createLegend(),
            enabled: false,
            getMax: function (t) { return (data == null ? -Infinity : data.GetLimits(t[0], t[1], 0)[1]); },
            getMin: function (t) { return (data == null ? Infinity : data.GetLimits(t[0], t[1], 0)[0]); },
        });
        setGuid(id);
        return function () { context.RemoveData(id); };
    }, []);
    return (React.createElement("g", null, data == null ? null :
        data.GetFullData().map(function (pt, i) {
            var _a;
            var barTop = context.YTransformation(pt[1] + ((_a = props.binSize) !== null && _a !== void 0 ? _a : 0), GraphContext_1.AxisMap.get(props.axis));
            var value = 1 - (pt[2] - zLimits[0]) / (zLimits[1] - zLimits[0]);
            var color = (0, helper_functions_1.HsvToHex)(props.hue, props.saturation, value);
            return React.createElement("rect", { key: i, x: context.XTransformation(pt[0]) - allBarOffset, y: barTop, width: barWidth, height: Math.abs(barTop - (props.binSize !== undefined ? context.YTransformation(pt[1], GraphContext_1.AxisMap.get(props.axis)) : allBarBottoms)), fill: color, stroke: color });
        })));
}
exports["default"] = HeatMapChart;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSGVhdE1hcENoYXJ0LmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vc3JjL0hlYXRNYXBDaGFydC50c3giXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IjtBQUFBLHlHQUF5RztBQUN6RywyQkFBMkI7QUFDM0IsRUFBRTtBQUNGLHFFQUFxRTtBQUNyRSxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4RyxzR0FBc0c7QUFDdEcsd0ZBQXdGO0FBQ3hGLEVBQUU7QUFDRiwwQ0FBMEM7QUFDMUMsRUFBRTtBQUNGLHdHQUF3RztBQUN4Ryx3R0FBd0c7QUFDeEcsNEVBQTRFO0FBQzVFLEVBQUU7QUFDRiw4QkFBOEI7QUFDOUIsd0dBQXdHO0FBQ3hHLDBCQUEwQjtBQUMxQixtREFBbUQ7QUFDbkQsRUFBRTtBQUNGLHlHQUF5Rzs7Ozs7Ozs7Ozs7Ozs7Ozs7O0FBR3pHLDZCQUErQjtBQUMvQixtRUFBd0Q7QUFDeEQsK0NBQTZGO0FBQzdGLHlDQUFzQztBQUN0QywyQ0FBc0M7QUFrQnRDLFNBQVMsWUFBWSxDQUFDLEtBQWE7SUFDL0I7O01BRUU7SUFDSSxJQUFBLEtBQUEsT0FBa0IsS0FBSyxDQUFDLFFBQVEsQ0FBUyxFQUFFLENBQUMsSUFBQSxFQUEzQyxJQUFJLFFBQUEsRUFBRSxPQUFPLFFBQThCLENBQUM7SUFDN0MsSUFBQSxLQUFBLE9BQWtCLEtBQUssQ0FBQyxRQUFRLENBQWlCLElBQUksQ0FBQyxJQUFBLEVBQXJELElBQUksUUFBQSxFQUFFLE9BQU8sUUFBd0MsQ0FBQztJQUN2RCxJQUFBLEtBQUEsT0FBMEIsS0FBSyxDQUFDLFFBQVEsQ0FBUyxDQUFDLENBQUMsSUFBQSxFQUFsRCxRQUFRLFFBQUEsRUFBRSxXQUFXLFFBQTZCLENBQUM7SUFDMUQsSUFBTSxPQUFPLEdBQUcsS0FBSyxDQUFDLFVBQVUsQ0FBQywyQkFBWSxDQUFDLENBQUM7SUFFL0MsSUFBTSxhQUFhLEdBQUcsS0FBSyxDQUFDLE9BQU8sQ0FBUyxjQUFNLE9BQUEsT0FBTyxD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QUMsQ0FBQyxDQUFDLENBQUMsR0FBRyxDQUFDLE9BQU8sQ0FBQyxDQUFDLENBQUMsR0FBRyxPQUFPLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztZQUNuRSxJQUFNLEtBQUssR0FBRyxJQUFBLDJCQUFRLEVBQUMsS0FBSyxDQUFDLEdBQUcsRUFBRSxLQUFLLENBQUMsVUFBVSxFQUFFLEtBQUssQ0FBQyxDQUFDO1lBQzNELE9BQU8sOEJBQU0sR0FBRyxFQUFFLENBQUMsRUFBRSxDQUFDLEVBQUUsT0FBTyxDQUFDLGVBQWUsQ0FBQyxFQUFFLENBQUMsQ0FBQyxDQUFDLENBQUMsR0FBRyxZQUFZLEVBQUUsQ0FBQyxFQUFFLE1BQU0sRUFBRSxLQUFLLEVBQUUsUUFBUSxFQUNqRyxNQUFNLEVBQUUsSUFBSSxDQUFDLEdBQUcsQ0FBQyxNQUFNLEdBQUMsQ0FBQyxLQUFLLENBQUMsT0FBTyxLQUFLLFNBQVMsQ0FBQyxDQUFDLENBQUMsT0FBTyxDQUFDLGVBQWUsQ0FBQyxFQUFFLENBQUMsQ0FBQyxDQUFDLEVBQUUsc0JBQU8sQ0FBQyxHQUFHLENBQUMsS0FBSyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLGFBQWEsQ0FBQyxDQUFDLEVBQUUsSUFBSSxFQUFFLEtBQUssRUFBRSxNQUFNLEVBQUUsS0FBSyxHQUFHLENBQUE7UUFDbkssQ0FBQyxDQUFDLENBRU4sQ0FDUCxDQUFDO0FBQ04sQ0FBQztBQUVELGtCQUFlLFlBQVksQ0FBQyJ9

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/HighlightBox.js":
/*!********************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/HighlightBox.js ***!
  \********************************************************************/
/***/ ((__unused_webpack_module, exports, __webpack_require__) => {

"use strict";

// ******************************************************************************************************
//  HighlightBox.tsx - Gbtc
//
//  Copyright © 2024, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  09/03/2024 - Preston Crawford
//       Generated original version of source code.
//
// ******************************************************************************************************
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
var HighlightBox = function (props) {
    var _a, _b, _c;
    var context = React.useContext(GraphContext_1.GraphContext);
    var axisIndex = GraphContext_1.AxisMap.get(props.Axis);
    var y1 = (_a = props.StartY) !== null && _a !== void 0 ? _a : context.YDomain[axisIndex][0];
    var y2 = (_b = props.EndY) !== null && _b !== void 0 ? _b : context.YDomain[axisIndex][1];
    var xStart = context.XTransformation(props.XVals[0]);
    var xEnd = context.XTransformation(props.XVals[1]);
    var yStart = context.YTransformation(y1, axisIndex);
    var yEnd = context.YTransformation(y2, axisIndex);
    var boxHeight = Math.abs(yEnd - yStart);
    var boxWidth = Math.abs(xEnd - xStart);
    return (React.createElement("g", null,
        React.createElement("rect", { x: Math.min(xStart, xEnd), y: Math.min(yStart, yEnd), width: boxWidth, height: boxHeight, fill: props.Color, opacity: props.Opacity, stroke: (_c = props.Stroke) !== null && _c !== void 0 ? _c : "none" })));
};
exports["default"] = HighlightBox;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSGlnaGxpZ2h0Qm94LmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vc3JjL0hpZ2hsaWdodEJveC50c3giXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IjtBQUFBLHlHQUF5RztBQUN6RywyQkFBMkI7QUFDM0IsRUFBRTtBQUNGLHFFQUFxRTtBQUNyRSxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4RyxzR0FBc0c7QUFDdEcsd0ZBQXdGO0FBQ3hGLEVBQUU7QUFDRiwwQ0FBMEM7QUFDMUMsRUFBRTtBQUNGLHdHQUF3RztBQUN4Ryx3R0FBd0c7QUFDeEcsNEVBQTRFO0FBQzVFLEVBQUU7QUFDRiw4QkFBOEI7QUFDOUIsd0dBQXdHO0FBQ3hHLGlDQUFpQztBQUNqQyxtREFBbUQ7QUFDbkQsRUFBRTtBQUNGLHlHQUF5Rzs7QUFFekcsNkJBQStCO0FBQy9CLCtDQUF1RTtBQVl2RSxJQUFNLFlBQVksR0FBRyxVQUFDLEtBQWE7O0lBQy9CLElBQU0sT0FBTyxHQUFHLEtBQUssQ0FBQyxVQUFVLENBQUMsMkJBQVksQ0FBQyxDQUFDO0lBQy9DLElBQU0sU0FBUyxHQUFHLHNCQUFPLENBQUMsR0FBRyxDQUFDLEtBQUssQ0FBQyxJQUFJLENBQUMsQ0FBQztJQUUxQyxJQUFNLEVBQUUsR0FBRyxNQUFBLEtBQUssQ0FBQyxNQUFNLG1DQUFJLE9BQU8sQ0FBQyxPQUFPLENBQUMsU0FBUyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7SUFDekQsSUFBTSxFQUFFLEdBQUcsTUFBQSxLQUFLLENBQUMsSUFBSSxtQ0FBSSxPQUFPLENBQUMsT0FBTyxDQUFDLFNBQVMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDO0lBRXZELElBQU0sTUFBTSxHQUFHLE9BQU8sQ0FBQyxlQUFlLENBQUMsS0FBSyxDQUFDLEtBQUssQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDO0lBQ3ZELElBQU0sSUFBSSxHQUFHLE9BQU8sQ0FBQyxlQUFlLENBQUMsS0FBSyxDQUFDLEtBQUssQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDO0lBRXJELElBQU0sTUFBTSxHQUFHLE9BQU8sQ0FBQyxlQUFlLENBQUMsRUFBRSxFQUFFLFNBQVMsQ0FBQyxDQUFDO0lBQ3RELElBQU0sSUFBSSxHQUFHLE9BQU8sQ0FBQyxlQUFlLENBQUMsRUFBRSxFQUFFLFNBQVMsQ0FBQyxDQUFDO0lBRXBELElBQU0sU0FBUyxHQUFHLElBQUksQ0FBQyxHQUFHLENBQUMsSUFBSSxHQUFHLE1BQU0sQ0FBQyxDQUFDO0lBQzFDLElBQU0sUUFBUSxHQUFHLElBQUksQ0FBQyxHQUFHLENBQUMsSUFBSSxHQUFHLE1BQU0sQ0FBQyxDQUFDO0lBRXpDLE9BQU8sQ0FDSDtRQUNJLDhCQUNJLENBQUMsRUFBRSxJQUFJLENBQUMsR0FBRyxDQUFDLE1BQU0sRUFBRSxJQUFJLENBQUMsRUFDekIsQ0FBQyxFQUFFLElBQUksQ0FBQyxHQUFHLENBQUMsTUFBTSxFQUFFLElBQUksQ0FBQyxFQUN6QixLQUFLLEVBQUUsUUFBUSxFQUNmLE1BQU0sRUFBRSxTQUFTLEVBQ2pCLElBQUksRUFBRSxLQUFLLENBQUMsS0FBSyxFQUNqQixPQUFPLEVBQUUsS0FBSyxDQUFDLE9BQU8sRUFDdEIsTUFBTSxFQUFFLE1BQUEsS0FBSyxDQUFDLE1BQU0sbUNBQUksTUFBTSxHQUNoQyxDQUNGLENBQ1AsQ0FBQztBQUNOLENBQUMsQ0FBQztBQUVGLGtCQUFlLFlBQVksQ0FBQyJ9

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/HorizontalMarker.js":
/*!************************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/HorizontalMarker.js ***!
  \************************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

// ******************************************************************************************************
//  HorizontalMarker.tsx - Gbtc
//
//  Copyright © 2022, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  04/29/2022 - C Lackner
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
function HorizontalMarker(props) {
    /*
      Marks a Y Value horizontally as a line.
    */
    var context = React.useContext(GraphContext_1.GraphContext);
    var _a = __read(React.useState(props.Value), 2), value = _a[0], setValue = _a[1];
    var _b = __read(React.useState(false), 2), isSelected = _b[0], setSelected = _b[1];
    var _c = __read(React.useState(""), 2), guid = _c[0], setGuid = _c[1];
    function generateData(v) {
        var axis = GraphContext_1.AxisMap.get(props.axis);
        var x1 = (props.start === undefined ? context.XDomain[0] : props.start);
        var x2 = (props.end === undefined ? context.XDomain[1] : props.end);
        return "M ".concat(context.XTransformation(x1), " ").concat(context.YTransformation(v, axis), " L ").concat(context.XTransformation(x2), " ").concat(context.YTransformation(v, axis));
    }
    var onClick = React.useCallback(function (_, y) {
        var axis = GraphContext_1.AxisMap.get(props.axis);
        var yP = context.YTransformation(props.Value, axis);
        var yT = context.YTransformation(y, axis);
        if (yT <= yP + (props.width / 2) && yT >= yP - (props.width / 2))
            setSelected(true);
    }, [props.width, props.Value, props.axis, context.YTransformation]);
    React.useEffect(function () {
        var id = context.RegisterSelect({
            axis: props.axis,
            allowSnapping: false,
            onClick: onClick,
            onRelease: function (_) { return setSelected(false); },
            onPlotLeave: function (_) { return setSelected(false); }
        });
        setGuid(id);
        return function () { context.RemoveSelect(id); };
    }, []);
    React.useEffect(function () {
        if (guid === "")
            return;
        context.UpdateSelect(guid, {
            axis: props.axis,
            allowSnapping: false,
            onClick: onClick,
            onRelease: function (_) { return setSelected(false); },
            onPlotLeave: function (_) { return setSelected(false); }
        });
    }, [onClick]);
    React.useEffect(function () {
        setValue(props.Value);
    }, [props.Value]);
    React.useEffect(function () {
        if (props.setValue === undefined)
            return;
        if (!isSelected && props.Value !== value)
            props.setValue(value);
    }, [isSelected, value]);
    React.useEffect(function () {
        if (context.CurrentMode !== 'select')
            setSelected(false);
    }, [context.CurrentMode]);
    React.useEffect(function () {
        if (isSelected)
            setValue(context.YHoverSnap[GraphContext_1.AxisMap.get(props.axis)]);
    }, [context.YHoverSnap, props.axis]);
    return (React.createElement("g", null,
        React.createElement("path", { d: generateData(props.Value), style: { fill: 'none', strokeWidth: props.width, stroke: props.color }, strokeDasharray: GraphContext_1.LineMap.get(props.lineStyle) }),
        props.setValue !== undefined && props.Value !== value && isSelected ?
            React.createElement("path", { d: generateData(value), style: { fill: 'none', strokeWidth: props.width, stroke: props.color, opacity: 0.5 }, strokeDasharray: GraphContext_1.LineMap.get(props.lineStyle) })
            : null));
}
exports["default"] = HorizontalMarker;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSG9yaXpvbnRhbE1hcmtlci5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uL3NyYy9Ib3Jpem9udGFsTWFya2VyLnRzeCJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiO0FBQUEseUdBQXlHO0FBQ3pHLCtCQUErQjtBQUMvQixFQUFFO0FBQ0YscUVBQXFFO0FBQ3JFLEVBQUU7QUFDRix3R0FBd0c7QUFDeEcsd0dBQXdHO0FBQ3hHLHNHQUFzRztBQUN0Ryx3RkFBd0Y7QUFDeEYsRUFBRTtBQUNGLDBDQUEwQztBQUMxQyxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4Ryw0RUFBNEU7QUFDNUUsRUFBRTtBQUNGLDhCQUE4QjtBQUM5Qix3R0FBd0c7QUFDeEcsMEJBQTBCO0FBQzFCLG1EQUFtRDtBQUNuRCxFQUFFO0FBQ0YseUdBQXlHOzs7Ozs7Ozs7Ozs7Ozs7Ozs7QUFHekcsNkJBQStCO0FBQy9CLCtDQUFvRztBQWFwRyxTQUFTLGdCQUFnQixDQUFDLEtBQWE7SUFDckM7O01BRUU7SUFDRixJQUFNLE9BQU8sR0FBRyxLQUFLLENBQUMsVUFBVSxDQUFDLDJCQUFZLENBQUMsQ0FBQTtJQUN4QyxJQUFBLEtBQUEsT0FBb0IsS0FBSyxDQUFDLFFBQVEsQ0FBUyxLQUFLLENBQUMsS0FBSyxDQUFDLElBQUEsRUFBdEQsS0FBSyxRQUFBLEVBQUUsUUFBUSxRQUF1QyxDQUFDO0lBQ3hELElBQUEsS0FBQSxPQUE0QixLQUFLLENBQUMsUUFBUSxDQUFVLEtBQUssQ0FBQyxJQUFBLEVBQXpELFVBQVUsUUFBQSxFQUFFLFdBQVcsUUFBa0MsQ0FBQztJQUMzRCxJQUFBLEtBQUEsT0FBa0IsS0FBSyxDQUFDLFFBQVEsQ0FBUyxFQUFFLENBQUMsSUFBQSxFQUEzQyxJQUFJLFFBQUEsRUFBRSxPQUFPLFFBQThCLENBQUM7SUFFbkQsU0FBUyxZQUFZLENBQUMsQ0FBUztRQUM3QixJQUFNLElBQUksR0FBRyxzQkFBTyxDQUFDLEdBQUcsQ0FBQyxLQUFLLENBQUMs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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/Infobox.js":
/*!***************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/Infobox.js ***!
  \***************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";
/* provided dependency */ var console = __webpack_require__(/*! console-browserify */ "./node_modules/console-browserify/index.js");

// ******************************************************************************************************
//  Infobox.tsx - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  06/16/2023 - G Santos
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
var Infobox = function (props) {
    var context = React.useContext(GraphContext_1.GraphContext);
    var _a = __read(React.useState(false), 2), isSelected = _a[0], setSelected = _a[1];
    var _b = __read(React.useState({ x: props.x, y: props.y }), 2), position = _b[0], setPosition = _b[1];
    var _c = __read(React.useState({ width: 100, height: 100 }), 2), dimension = _c[0], setDimensions = _c[1];
    var _d = __read(React.useState(""), 2), guid = _d[0], setGuid = _d[1];
    var offsetDefault = 0;
    // Functions
    var calculateX = React.useCallback(function (xArg) {
        var _a, _b, _c;
        var x = ((_a = props.usePixelPositioning) !== null && _a !== void 0 ? _a : false) ? context.XApplyPixelOffset(xArg) : context.XTransformation(xArg);
        // Convert x/y to upper-left corner
        switch (props.origin) {
            case "lower-right":
            case "upper-right": {
                x -= (dimension.width + ((_b = props.offset) !== null && _b !== void 0 ? _b : offsetDefault));
                break;
            }
            case "lower-center":
            case "upper-center": {
                x -= Math.floor(dimension.width / 2);
                break;
            }
            // Do-nothing case
            case undefined:
            case "lower-left":
            case "upper-left":
                x += (_c = props.offset) !== null && _c !== void 0 ? _c : offsetDefault;
                break;
        }
        return x;
    }, [context.XApplyPixelOffset, context.XTransformation, props.origin, props.offset, props.usePixelPositioning, dimension]);
    var calculateY = React.useCallback(function (yArg) {
        var _a, _b, _c;
        var y = ((_a = props.usePixelPositioning) !== null && _a !== void 0 ? _a : false) ? context.YApplyPixelOffset(yArg) : context.YTransformation(yArg, GraphContext_1.AxisMap.get(props.axis));
        // Convert x/y to upper-left corner
        switch (props.origin) {
            case undefined:
            case "upper-left":
            case "upper-right":
            case "upper-center":
                y += (_b = props.offset) !== null && _b !== void 0 ? _b : offsetDefault;
                break;
            case "lower-left":
            case "lower-right":
            case "lower-center":
                y -= (dimension.height + ((_c = props.offset) !== null && _c !== void 0 ? _c : offsetDefault));
                break;
        }
        return y;
    }, [context.YApplyPixelOffset, context.YTransformation, props.origin, props.offset, props.usePixelPositioning, props.axis, dimension]);
    var onClick = React.useCallback(function (xArg, yArg) {
        var xP = calculateX(props.x);
        var xT = context.XTransformation(xArg);
        var yP = calculateY(props.y);
        var yT = context.YTransformation(yArg, GraphContext_1.AxisMap.get(props.axis));
        if (xT <= xP + dimension.width && xT >= xP && yT <= yP + dimension.height && yT >= yP) {
            setSelected(true);
        }
    }, [props.x, props.y, calculateX, calculateY, dimension, setSelected, context.XTransformation, context.YTransformation, props.axis]);
    // Note: this is the only function not effected by usePixelPositioning
    var onMove = props.onMouseMove === undefined ? undefined : React.useCallback(function (xArg, yArg) {
        if (props.onMouseMove !== undefined)
            props.onMouseMove(xArg, yArg);
    }, [props.onMouseMove]);
    // useEffect
    React.useEffect(function () {
        var id = context.RegisterSelect({
            axis: props.axis,
            allowSnapping: false,
            onRelease: function (_) { return setSelected(false); },
            onPlotLeave: function (_) { return setSelected(false); },
            onClick: onClick,
            onMove: onMove
        });
        setGuid(id);
        return function () { context.RemoveSelect(id); };
    }, []);
    React.useEffect(function () {
        if (guid === "")
            return;
        context.UpdateSelect(guid, {
            axis: props.axis,
            allowSnapping: false,
            onRelease: function (_) { return setSelected(false); },
            onPlotLeave: function (_) { return setSelected(false); },
            onClick: onClick,
            onMove: onMove
        });
    }, [onClick, onMove, props.axis]);
    React.useEffect(function () {
        setPosition({ x: props.x, y: props.y });
    }, [props.x, props.y]);
    React.useEffect(function () {
        if (props.setPosition === undefined)
            return;
        if (!isSelected && (props.x !== position.x || props.y !== position.y))
            props.setPosition(position.x, position.y);
    }, [isSelected, position]);
    React.useEffect(function () {
        if (context.CurrentMode !== 'select')
            setSelected(false);
    }, [context.CurrentMode]);
    React.useEffect(function () {
        var _a;
        if (isSelected && !((_a = props.disallowSnapping) !== null && _a !== void 0 ? _a : false))
            setPosition({ x: context.XHoverSnap, y: context.YHoverSnap[GraphContext_1.AxisMap.get(props.axis)] });
    }, [context.XHoverSnap, context.YHoverSnap, props.axis]);
    React.useEffect(function () {
        var _a;
        if (isSelected && ((_a = props.disallowSnapping) !== null && _a !== void 0 ? _a : false))
            setPosition({ x: context.XHover, y: context.YHover[GraphContext_1.AxisMap.get(props.axis)] });
    }, [context.XHover, context.YHover, props.axis]);
    // Get Heights and Widths
    React.useEffect(function () {
        var domEle = document.getElementById(props.childId);
        if (domEle == null) {
            console.error("Invalid element id passed for child element in infobox ".concat(props.childId));
            setDimensions({ width: 100, height: 100 });
            return;
        }
        if (dimension.width === Math.ceil(domEle.clientWidth) && dimension.height === Math.ceil(domEle.clientHeight))
            return;
        setDimensions({ width: Math.ceil(domEle.clientWidth), height: Math.ceil(domEle.clientHeight) });
    }, [props.children, props.childId]);
    return (React.createElement("g", null,
        React.createElement(InfoGraphic, { x: calculateX(props.x), y: calculateY(props.y), width: dimension.width, height: dimension.height, opacity: props.opacity }),
        React.createElement("foreignObject", { x: calculateX(props.x), y: calculateY(props.y), width: dimension.width, height: dimension.height }, props.children),
        props.setPosition !== undefined && (props.x !== position.x || props.y !== position.y) ?
            React.createElement(InfoGraphic, { x: calculateX(position.x), y: calculateY(position.y), width: dimension.width, height: dimension.height, opacity: props.opacity })
            : null));
};
var InfoGraphic = function (props) {
    var _a;
    return (React.createElement("path", { d: "M ".concat(props.x, " ").concat(props.y, " h ").concat(props.width, " v ").concat(props.height, " h -").concat(props.width, " v -").concat(props.height), stroke: 'black', style: { opacity: (_a = props.opacity) !== null && _a !== void 0 ? _a : 1 } }));
};
exports["default"] = Infobox;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSW5mb2JveC5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uL3NyYy9JbmZvYm94LnRzeCJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiO0FBQUEseUdBQXlHO0FBQ3pHLHNCQUFzQjtBQUN0QixFQUFFO0FBQ0YscUVBQXFFO0FBQ3JFLEVBQUU7QUFDRix3R0FBd0c7QUFDeEcsd0dBQXdHO0FBQ3hHLHNHQUFzRztBQUN0Ryx3RkFBd0Y7QUFDeEYsRUFBRTtBQUNGLDBDQUEwQztBQUMxQyxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4Ryw0RUFBNEU7QUFDNUUsRUFBRTtBQUNGLDhCQUE4QjtBQUM5Qix3R0FBd0c7QUFDeEcseUJBQXlCO0FBQ3pCLG1EQUFtRDtBQUNuRCxFQUFFO0FBQ0YseUdBQXlHOzs7Ozs7Ozs7Ozs7Ozs7Ozs7QUFFekcsNkJBQStCO0FBQy9CLCtDQUFrRjtBQW9CbEYsSUFBTSxPQUFPLEdBQW9DLFVBQUMsS0FBSztJQUNyRCxJQUFNLE9BQU8sR0FBRyxLQUFLLENBQUMsVUFBVSxDQUFDLDJCQUFZLENBQUMsQ0FBQztJQUN6QyxJQUFBLEtBQUEsT0FBNEIsS0FBSyxDQUFDLFFBQVEsQ0FBVSxLQUFLLENBQUMsSUFBQSxFQUF6RCxVQUFVLFFBQUEsRUFBRSxXQUFXLFFBQWtDLENBQUM7SUFDM0QsSUFBQSxLQUFBLE9BQTBCLEtBQUssQ0FBQyxRQUFRLENBQXlCLEVBQUMsQ0FBQyxFQUFFLEtBQUssQ0FBQyxDQUFDLEVBQUUsQ0FBQyxFQUFFLEtBQUssQ0FBQyxDQUFDLEVBQUMsQ0FBQyxJQUFBLEVBQXpGLFFBQVEsUUFBQSxFQUFFLFdBQVcsUUFBb0UsQ0FBQztJQUMzRixJQUFBLEtBQUEsT0FBNkIsS0FBSyxDQUFDLFFBQVEsQ0FBa0MsRUFBQyxLQUFLLEVBQUUsR0FBRyxFQUFFLE1BQU0sRUFBRSxHQUFHLEVBQUMsQ0FBQyxJQUFBLEVBQXRHLFNBQVMsUUFBQSxFQUFFLGFBQWEsUUFBOEUsQ0FBQztJQUN4RyxJQUFB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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/InteractiveButtons.js":
/*!**************************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/InteractiveButtons.js ***!
  \**************************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

// ******************************************************************************************************
//  InteractiveButtons.tsx - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  03/18/2021 - C Lackner
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var gpa_symbols_1 = __webpack_require__(/*! @gpa-gemstone/gpa-symbols */ "webpack/sharing/consume/default/@gpa-gemstone/gpa-symbols/@gpa-gemstone/gpa-symbols?6dfb");
var Button_1 = __webpack_require__(/*! ./Button */ "./node_modules/@gpa-gemstone/react-graph/lib/Button.js");
var heightPerButton = 25;
var InteractiveButtons = React.memo(function (props) {
    var _a, _b;
    var btnCleanup = React.useRef(undefined);
    var _c = __read(React.useState(React.createElement(React.Fragment, null, gpa_symbols_1.Point)), 2), selectIcon = _c[0], setSelectIcon = _c[1];
    var _d = __read(React.useState((_a = props.holdOpen) !== null && _a !== void 0 ? _a : false), 2), expand = _d[0], setExpand = _d[1];
    var _e = __read(React.useState(undefined), 2), currentSelect = _e[0], setCurrentSelect = _e[1];
    var _f = __read(React.useMemo(function () {
        var _a;
        var nButtons = (((_a = props.holdOpen) !== null && _a !== void 0 ? _a : false) ? 1 : 0) +
            (props.showZoom ? 3 : 0) +
            (props.showPan ? 1 : 0) +
            (props.showReset ? 1 : 0) +
            (props.showSelect ? 1 : 0) +
            (props.showDownload ? 1 : 0) +
            (props.showCapture ? 1 : 0) +
            ((props.children == null) ? 0 : React.Children.count(props.children));
        var buttonsAllowed = Math.floor((props.heightAvaliable - 20) / heightPerButton);
        var rows = Math.ceil(nButtons / buttonsAllowed);
        var width = 20 * rows;
        nButtons = Math.min(nButtons, buttonsAllowed);
        return [nButtons, rows, heightPerButton * (nButtons - 1), width];
    }, [props.holdOpen, props.showZoom, props.showPan, props.showReset, props.showSelect, props.showDownload, props.showCapture, props.children]), 4), nButtons = _f[0], nRows = _f[1], height = _f[2], width = _f[3];
    var setBtnAndSelect = React.useCallback(function (newIcon, id) {
        setSelectIcon(newIcon);
        setCurrentSelect(id);
        props.setSelection('select');
        collaspeMenu();
    }, [props.setSelection]);
    var openTray = React.useCallback(function (evt) {
        evt.stopPropagation();
        setExpand(true);
    }, [setExpand]);
    var collaspeMenu = React.useCallback(function () {
        var _a;
        if (!((_a = props.holdOpen) !== null && _a !== void 0 ? _a : false))
            setExpand(false);
    }, [props.holdOpen]);
    var displayIcon = React.useMemo(function () {
        switch (props.currentSelection) {
            default:
            case 'pan': return gpa_symbols_1.Pan;
            case 'zoom-rectangular': return gpa_symbols_1.MagnifyingGlass;
            case 'zoom-vertical': return "\u2016";
            case 'zoom-horizontal': return "\u2550";
            case 'select': return selectIcon;
        }
    }, [selectIcon, props.currentSelection]);
    React.useEffect(function () { if (expand)
        props.setWidth(width);
    else
        props.setWidth(20); }, [width, expand]);
    if (nButtons === 0)
        return null;
    if (nButtons === 1 || !expand)
        return (React.createElement("g", null,
            React.createElement("circle", { stroke: 'black', onClick: openTray, r: 10, cx: props.x, cy: props.y, style: { fill: '#002eff', pointerEvents: 'all' }, onMouseDown: function (evt) { return evt.stopPropagation(); }, onMouseUp: function (evt) { return evt.stopPropagation(); } }),
            React.createElement("text", { fill: 'black', style: { fontSize: '1em', textAnchor: 'middle', dominantBaseline: 'middle' }, x: props.x, y: props.y }, displayIcon)));
    var symbols = [[]];
    var symbolNames = [[]];
    if ((_b = props.holdOpen) !== null && _b !== void 0 ? _b : false) {
        if (symbols[symbols.length - 1].length < nButtons) {
            symbols[symbols.length - 1].push(React.createElement(Button_1.default, { onClick: function () { return setExpand(false); } }, "^"));
            symbolNames[symbols.length - 1].push('collaspe');
        }
        else {
            symbols.push([React.createElement(Button_1.default, { onClick: function () { return setExpand(false); } }, "^")]);
            symbolNames.push(['collaspe']);
        }
    }
    if (props.showZoom) {
        if (symbols[symbols.length - 1].length < nButtons) {
            symbolNames[symbols.length - 1].push('zoom-rectangular');
            symbols[symbols.length - 1].push(React.createElement(Button_1.default, { onClick: function () { props.setSelection('zoom-rectangular'); collaspeMenu(); } }, gpa_symbols_1.MagnifyingGlass));
        }
        else {
            symbolNames.push(['zoom-rectangular']);
            symbols.push([React.createElement(Button_1.default, { onClick: function () { props.setSelection('zoom-rectangular'); collaspeMenu(); } }, gpa_symbols_1.MagnifyingGlass)]);
        }
        if (symbols[symbols.length - 1].length < nButtons) {
            symbolNames[symbols.length - 1].push('zoom-vertical');
            symbols[symbols.length - 1].push(React.createElement(Button_1.default, { onClick: function () { props.setSelection('zoom-vertical'); collaspeMenu(); } }, "\u2016"));
        }
        else {
            symbolNames.push(['zoom-vertical']);
            symbols.push([React.createElement(Button_1.default, { onClick: function () { props.setSelection('zoom-vertical'); collaspeMenu(); } }, "\u2016")]);
        }
        if (symbols[symbols.length - 1].length < nButtons) {
            symbolNames[symbols.length - 1].push('zoom-horizontal');
            symbols[symbols.length - 1].push(React.createElement(Button_1.default, { onClick: function () { props.setSelection('zoom-horizontal'); collaspeMenu(); } }, "\u2550"));
        }
        else {
            symbolNames.push(['zoom-horizontal']);
            symbols.push([React.createElement(Button_1.default, { onClick: function () { props.setSelection('zoom-horizontal'); collaspeMenu(); } }, "\u2550")]);
        }
    }
    if (props.showPan && symbols[symbols.length - 1].length < nButtons) {
        symbolNames[symbols.length - 1].push('pan');
        symbols[symbols.length - 1].push(React.createElement(Button_1.default, { onClick: function () { props.setSelection('pan'); collaspeMenu(); } }, gpa_symbols_1.Pan));
    }
    else if (props.showPan) {
        symbolNames.push(['pan']);
        symbols.push([React.createElement(Button_1.default, { onClick: function () { props.setSelection('pan'); collaspeMenu(); } }, gpa_symbols_1.Pan)]);
    }
    if (props.showSelect && symbols[symbols.length - 1].length < nButtons) {
        symbolNames[symbols.length - 1].push('select');
        symbols[symbols.length - 1].push(React.createElement(Button_1.default, { isSelect: true, onClick: function () { setBtnAndSelect(React.createElement(React.Fragment, null, gpa_symbols_1.Point), 'select'); } }, gpa_symbols_1.Point));
    }
    else if (props.showSelect) {
        symbolNames.push(['select']);
        symbols.push([React.createElement(Button_1.default, { isSelect: true, onClick: function () { setBtnAndSelect(React.createElement(React.Fragment, null, gpa_symbols_1.Point), 'select'); } }, gpa_symbols_1.Point)]);
    }
    if (props.showReset && symbols[symbols.length - 1].length < nButtons) {
        symbolNames[symbols.length - 1].push('reset');
        symbols[symbols.length - 1].push(React.createElement(Button_1.default, { onClick: function () { collaspeMenu(); props.setSelection('reset'); } }, gpa_symbols_1.House));
    }
    else if (props.showReset) {
        symbolNames.push(['reset']);
        symbols.push([React.createElement(Button_1.default, { onClick: function () { collaspeMenu(); props.setSelection('reset'); } }, gpa_symbols_1.House)]);
    }
    if (props.showDownload && symbols[symbols.length - 1].length < nButtons) {
        symbolNames[symbols.length - 1].push('download');
        symbols[symbols.length - 1].push(React.createElement(Button_1.default, { onClick: function () { collaspeMenu(); props.setSelection('download'); } }, gpa_symbols_1.InputNumbers));
    }
    else if (props.showDownload) {
        symbolNames.push(['download']);
        symbols.push([React.createElement(Button_1.default, { onClick: function () { collaspeMenu(); props.setSelection('download'); } }, gpa_symbols_1.InputNumbers)]);
    }
    if (props.showCapture && symbols[symbols.length - 1].length < nButtons) {
        symbolNames[symbols.length - 1].push('capture');
        symbols[symbols.length - 1].push(React.createElement(Button_1.default, { onClick: function () { collaspeMenu(); props.setSelection('capture'); } }, gpa_symbols_1.Scroll));
    }
    else if (props.showCapture) {
        symbolNames.push(['capture']);
        symbols.push([React.createElement(Button_1.default, { onClick: function () { collaspeMenu(); props.setSelection('capture'); } }, gpa_symbols_1.Scroll)]);
    }
    var customButtonsIndex = symbols.length - 1;
    React.Children.forEach(props.children, function (element, index) {
        if (symbols[symbols.length - 1].length < nButtons && React.isValidElement(element) && element.type === Button_1.default) {
            symbols[symbols.length - 1].push(element);
            symbolNames[symbols.length - 1].push('custom-' + index);
        }
        else if (React.isValidElement(element) && element.type === Button_1.default) {
            symbols.push([element]);
            symbolNames.push(['custom-' + index]);
        }
    });
    var path = "M ".concat(props.x - 10, " ").concat(props.y, " A 10 10 90 0 1 ").concat(props.x, " ").concat(props.y - 10, " h ").concat(width - 20, " A 10 10 90 0 1 ").concat(props.x + width - 10, " ").concat(props.y, " v ").concat(height, " A 10 10 90 0 1 ").concat(props.x + width - 20, " ").concat(props.y + height + 10, " h ").concat(-width + 20, " A 10 10 90 0 1 ").concat(props.x - 10, " ").concat(props.y + height, " v ").concat(-height);
    return (React.createElement("g", { style: { cursor: 'default' }, "data-html2canvas-ignore": "true" },
        React.createElement("path", { d: path, style: {
                fill: '#1e90ff'
            } }),
        symbols.map(function (r, j) { return React.createElement(React.Fragment, null,
            " ",
            r.map(function (s, i) {
                return React.createElement(CircleButton, { key: i, selectId: symbolNames[j][i], x: props.x + j * 20, y: props.y + i * heightPerButton, active: i < customButtonsIndex ? (props.currentSelection === symbolNames[j][i] && (props.currentSelection !== 'select' || currentSelect === undefined)) :
                        props.currentSelection === 'select' && currentSelect === symbolNames[j][i], button: s, btnCleanup: btnCleanup, setSelectIcon: !symbolNames[j][i].startsWith('custom') ? undefined : setBtnAndSelect });
            })); }),
        React.createElement("path", { d: path, stroke: 'black' })));
});
function CircleButton(props) {
    return (React.createElement(React.Fragment, null,
        React.createElement("circle", { r: 10, cx: props.x, cy: props.y, style: { fill: (props.active ? '#002eff' : '#1e90ff'), pointerEvents: 'all' }, onMouseDown: function (evt) { return evt.stopPropagation(); }, onClick: function (evt) {
                var _a;
                evt.stopPropagation();
                if ((props.setSelectIcon !== undefined) && ((_a = props.button.props.isSelect) !== null && _a !== void 0 ? _a : false))
                    props.setSelectIcon(props.button.props.children, props.selectId);
                if (props.btnCleanup.current !== undefined)
                    props.btnCleanup.current();
                props.btnCleanup.current = props.button.props.onClick();
            }, onMouseUp: function (evt) { return evt.stopPropagation(); } }),
        React.createElement("text", { fill: 'black', style: { fontSize: '1em', textAnchor: 'middle', dominantBaseline: 'middle' }, x: props.x, y: props.y }, props.button)));
}
exports["default"] = InteractiveButtons;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSW50ZXJhY3RpdmVCdXR0b25zLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vc3JjL0ludGVyYWN0aXZlQnV0dG9ucy50c3giXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IjtBQUFBLHlHQUF5RztBQUN6RyxpQ0FBaUM7QUFDakMsRUFBRTtBQUNGLHFFQUFxRTtBQUNyRSxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4RyxzR0FBc0c7QUFDdEcsd0ZBQXdGO0FBQ3hGLEVBQUU7QUFDRiwwQ0FBMEM7QUFDMUMsRUFBRTtBQUNGLHdHQUF3RztBQUN4Ryx3R0FBd0c7QUFDeEcsNEVBQTRFO0FBQzVFLEVBQUU7QUFDRiw4QkFBOEI7QUFDOUIsd0dBQXdHO0FBQ3hHLDBCQUEwQjtBQUMxQixtREFBbUQ7QUFDbkQsRUFBRTtBQUNGLHlHQUF5Rzs7Ozs7Ozs7Ozs7Ozs7Ozs7O0FBRXpHLDZCQUErQjtBQUMvQix5REFBa0c7QUFFbEcsbUNBQTZCO0FBc0I3QixJQUFNLGVBQWUsR0FBRyxFQUFFLENBQUM7QUFFM0IsSUFBTSxrQkFBa0IsR0FBRyxLQUFLLENBQUMsSUFBSSxDQUFDLFVBQUMsS0FBYTs7SUFDaEQsSUFBTSxVQUFVLEdBQUcsS0FBSyxDQUFDLE1BQU0sQ0FBVSxTQUFTLENBQUMsQ0FBQztJQUM5QyxJQUFBLEtBQUEsT0FBOEIsS0FBSyxDQUFDLFFBQVEsQ0FBcUIsMENBQUcsbUJBQUssQ0FBSSxDQUFDLElBQUEsRUFBN0UsVUFBVSxRQUFBLEVBQUUsYUFBYSxRQUFvRCxDQUFDO0lBQy9FLElBQUEsS0FBQSxPQUFzQixLQUFLLENBQUMsUUFBUSxDQUFVLE1BQUEsS0FBSyxDQUFDLFFBQVEsbUNBQUksS0FBSyxDQUFDLElBQUEsRUFBckUsTUFBTSxRQUFBLEVBQUUsU0FBUyxRQUFvRCxDQUFDO0lBQ3ZFLElBQUEsS0FBQSxPQUFvQyxLQUFLLENBQUMsUUFBUSxDQUFnQyxTQUFTLENBQUMsSUFBQSxFQUEzRixhQUFhLFFBQUEsRUFBRSxn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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/Legend.js":
/*!**************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/Legend.js ***!
  \**************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

// ******************************************************************************************************
//  Legend.tsx - Gbtc
//
//  Copyright © 2021, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  03/19/2021 - C. lackner
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
var LegendContext_1 = __webpack_require__(/*! ./LegendContext */ "./node_modules/@gpa-gemstone/react-graph/lib/LegendContext.js");
var itemHeight = 25;
var itemsWhenBottom = 3;
function Legend(props) {
    var graphContext = React.useContext(GraphContext_1.GraphContext);
    var _a = __read(React.useState(props.location === 'bottom' ? props.graphWidth : props.width), 2), width = _a[0], setWidth = _a[1];
    var _b = __read(React.useState(props.location === 'right' ? props.graphHeight : props.height), 2), height = _b[0], setHeight = _b[1];
    var _c = __read(React.useState({ sm: 0, lg: 0 }), 2), nLegends = _c[0], setNLegends = _c[1];
    var _d = __read(React.useState(false), 2), hasScroll = _d[0], setHasScroll = _d[1];
    var _e = __read(React.useState(0), 2), leftPad = _e[0], setLeftPad = _e[1];
    var legendContextValue = React.useMemo(function () {
        var scrollBarSpace = (hasScroll ? 6 : 0);
        var baseWidth = width - leftPad;
        var baseHeight = props.location === 'bottom' ? itemHeight : Math.max(height / (Math.max(nLegends.sm + nLegends.lg, 1)), itemHeight);
        return {
            SmWidth: (baseWidth / (props.location === 'bottom' ? itemsWhenBottom : 1)) - scrollBarSpace,
            LgWidth: baseWidth - scrollBarSpace,
            SmHeight: baseHeight,
            LgHeight: baseHeight * (props.location === 'bottom' ? 2 : 1),
            RequestLegendWidth: props.RequestLegendWidth,
            RequestLegendHeight: props.RequestLegendHeight
        };
    }, [width, height, props.RequestLegendWidth, props.RequestLegendHeight, hasScroll, props.location, leftPad, nLegends]);
    React.useEffect(function () {
        var newWidth = props.location === 'bottom' ? props.graphWidth : props.width;
        if (newWidth !== width)
            setWidth(newWidth);
    }, [props.width, props.graphWidth, props.location]);
    React.useEffect(function () {
        var newHeight = props.location === 'right' ? props.graphHeight : props.height;
        if (newHeight !== height)
            setHeight(newHeight);
    }, [props.height, props.graphHeight, props.location]);
    React.useEffect(function () {
        var newNLegends = __spreadArray([], __read(graphContext.Data.current.values()), false).reduce(function (s, c) {
            var _a, _b, _c, _d, _e, _f;
            if (c.legend === undefined)
                return s;
            if (props.HideDisabled && !((_c = (_b = (_a = c.legend) === null || _a === void 0 ? void 0 : _a.props) === null || _b === void 0 ? void 0 : _b.enabled) !== null && _c !== void 0 ? _c : true))
                return s;
            if (((_f = (_e = (_d = c.legend) === null || _d === void 0 ? void 0 : _d.props) === null || _e === void 0 ? void 0 : _e.size) !== null && _f !== void 0 ? _f : 'sm') === 'sm')
                s.sm = s.sm + 1;
            else
                s.lg = s.lg + 1;
            return s;
        }, { sm: 0, lg: 0 });
        if (newNLegends.sm !== nLegends.sm || newNLegends.lg !== nLegends.lg)
            setNLegends(newNLegends);
    }, [graphContext.DataGuid, props.HideDisabled]);
    React.useEffect(function () {
        var requiredHeight = Math.ceil(nLegends.sm / (props.location === 'bottom' ? itemsWhenBottom : 1)) * legendContextValue.SmHeight + nLegends.lg * legendContextValue.LgHeight;
        if (props.RequestLegendHeight !== undefined && requiredHeight !== height)
            props.RequestLegendHeight(requiredHeight);
        setHasScroll(requiredHeight > height);
    }, [nLegends, props.location, height, props.RequestLegendHeight]);
    React.useEffect(function () { return setLeftPad(props.location === 'bottom' ? 39 : 0); }, [props.location]);
    return (React.createElement(LegendContext_1.LegendContext.Provider, { value: legendContextValue },
        React.createElement("div", { style: { height: height, width: width, paddingLeft: "".concat(leftPad, "px"), position: (props.location === 'bottom' ? 'absolute' : 'relative'), float: props.location, display: 'flex', flexWrap: 'wrap', bottom: 0,
                overflowY: hasScroll ? 'scroll' : 'hidden', overflowX: hasScroll ? 'visible' : 'hidden', cursor: 'default' } }, __spreadArray([], __read(graphContext.Data.current.values()), false).map(function (series, index) {
            var _a, _b;
            return (series.legend !== undefined && (!props.HideDisabled || ((_a = series.legend.props.enabled) !== null && _a !== void 0 ? _a : true)) ?
                React.createElement("div", { key: index, "data-html2canvas-ignore": !((_b = series.legend.props.enabled) !== null && _b !== void 0 ? _b : true) }, series.legend) : null);
        }))));
}
exports["default"] = React.memo(Legend);
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiTGVnZW5kLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vc3JjL0xlZ2VuZC50c3giXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IjtBQUFBLHlHQUF5RztBQUN6RyxxQkFBcUI7QUFDckIsRUFBRTtBQUNGLHFFQUFxRTtBQUNyRSxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4RyxzR0FBc0c7QUFDdEcsd0ZBQXdGO0FBQ3hGLEVBQUU7QUFDRiwwQ0FBMEM7QUFDMUMsRUFBRTtBQUNGLHdHQUF3RztBQUN4Ryx3R0FBd0c7QUFDeEcsNEVBQTRFO0FBQzVFLEVBQUU7QUFDRiw4QkFBOEI7QUFDOUIsd0dBQXdHO0FBQ3hHLDJCQUEyQjtBQUMzQixtREFBbUQ7QUFDbkQsRUFBRTtBQUNGLHlHQUF5Rzs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7O0FBRXpHLDZCQUErQjtBQUMvQiwrQ0FBNEM7QUFDNUMsaURBQWdFO0FBYWhFLElBQU0sVUFBVSxHQUFHLEVBQUUsQ0FBQztBQUN0QixJQUFNLGVBQWUsR0FBRyxDQUFDLENBQUM7QUFFMUIsU0FBUyxNQUFNLENBQUMsS0FBYTtJQUMzQixJQUFNLFlBQVksR0FBRyxLQUFLLENBQUMsVUFBVSxDQUFDLDJCQUFZLENBQUMsQ0FBQztJQUM5QyxJQUFBLEtBQUEsT0FBb0IsS0FBSyxDQUFDLFFBQVEsQ0FBUyxLQUFLLENBQUMsUUFBUSxLQUFLLFFBQVEsQ0FBQSxDQUFDLENBQUMsS0FBSyxDQUFDLFVBQVUsQ0FBQyxDQUFDLENBQUMsS0FBSyxDQUFDLEtBQUssQ0FBQyxJQUFBLEVBQXRHLEtBQUssUUFBQSxFQUFFLFFBQVEsUUFBdUYsQ0FBQztJQUN4RyxJQUFBLEtBQUEsT0FBc0IsS0FBSyxDQUFDLFFBQVEsQ0FBUyxLQUFLLENBQUMsUUFBUSxLQUFLLE9BQU8sQ0FBQSxDQUFDLENBQUMsS0FBSyxDQUFDLFdBQVcsQ0FBQyxDQUFDLENBQUMsS0FBSyxDQUFDLE1BQU0sQ0FBQyxJQUFBLEVBQXpHLE1B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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/LegendContext.js":
/*!*********************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/LegendContext.js ***!
  \*********************************************************************/
/***/ ((__unused_webpack_module, exports, __webpack_require__) => {

"use strict";

Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.LegendContext = void 0;
// ******************************************************************************************************
//  GraphContext.tsx - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  02/01/2024 - G. Santos
//       Generated original version of source code.
//
// ******************************************************************************************************
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
exports.LegendContext = React.createContext({
    SmWidth: 0,
    LgWidth: 0,
    SmHeight: 0,
    LgHeight: 0,
    RequestLegendWidth: function () { return undefined; },
    RequestLegendHeight: function () { return undefined; }
});
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiTGVnZW5kQ29udGV4dC5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uL3NyYy9MZWdlbmRDb250ZXh0LnRzeCJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiOzs7QUFBQSx5R0FBeUc7QUFDekcsMkJBQTJCO0FBQzNCLEVBQUU7QUFDRixxRUFBcUU7QUFDckUsRUFBRTtBQUNGLHdHQUF3RztBQUN4Ryx3R0FBd0c7QUFDeEcsc0dBQXNHO0FBQ3RHLHdGQUF3RjtBQUN4RixFQUFFO0FBQ0YsMENBQTBDO0FBQzFDLEVBQUU7QUFDRix3R0FBd0c7QUFDeEcsd0dBQXdHO0FBQ3hHLDRFQUE0RTtBQUM1RSxFQUFFO0FBQ0YsOEJBQThCO0FBQzlCLHdHQUF3RztBQUN4RywwQkFBMEI7QUFDMUIsbURBQW1EO0FBQ25ELEVBQUU7QUFDRix5R0FBeUc7QUFDekcsNkJBQStCO0FBZ0JsQixRQUFBLGFBQWEsR0FBRyxLQUFLLENBQUMsYUFBYSxDQUFDO0lBQzdDLE9BQU8sRUFBRSxDQUFDO0lBQ1YsT0FBTyxFQUFFLENBQUM7SUFDVixRQUFRLEVBQUUsQ0FBQztJQUNYLFFBQVEsRUFBRSxDQUFDO0lBQ1gsa0JBQWtCLEVBQUUsY0FBTSxPQUFBLFNBQVMsRUFBVCxDQUFTO0lBQ25DLG1CQUFtQixFQUFFLGNBQU0sT0FBQSxTQUFTLEVBQVQsQ0FBUztDQUNyQixDQUFDLENBQUMifQ==

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/Line.js":
/*!************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/Line.js ***!
  \************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

// ******************************************************************************************************
//  Line.tsx - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  03/18/2021 - C Lackner
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
var moment = __webpack_require__(/*! moment */ "webpack/sharing/consume/default/moment/moment?be5a");
var PointNode_1 = __webpack_require__(/*! ./PointNode */ "./node_modules/@gpa-gemstone/react-graph/lib/PointNode.js");
var LineLegend_1 = __webpack_require__(/*! ./LineLegend */ "./node_modules/@gpa-gemstone/react-graph/lib/LineLegend.js");
var helper_functions_1 = __webpack_require__(/*! @gpa-gemstone/helper-functions */ "./node_modules/@gpa-gemstone/helper-functions/lib/index.js");
function Line(props) {
    var _a;
    /*
        Single Line with ability to turn off and on.
    */
    var _b = __read(React.useState(""), 2), guid = _b[0], setGuid = _b[1];
    var _c = __read(React.useState(""), 2), dataGuid = _c[0], setDataGuid = _c[1];
    var _d = __read(React.useState([NaN, NaN]), 2), highlight = _d[0], setHighlight = _d[1];
    var _e = __read(React.useState(true), 2), enabled = _e[0], setEnabled = _e[1];
    var _f = __read(React.useState(null), 2), data = _f[0], setData = _f[1];
    var _g = __read(React.useState([]), 2), visibleData = _g[0], setVisibleData = _g[1];
    var context = React.useContext(GraphContext_1.GraphContext);
    var showPoints = React.useMemo(function () { return (props.showPoints !== undefined && props.showPoints) ||
        ((props.autoShowPoints === undefined || props.autoShowPoints) && visibleData.length <= 100); }, [props.showPoints, props.autoShowPoints, visibleData]);
    var createLegend = React.useCallback(function () {
        var _a;
        if (props.legend === undefined)
            return undefined;
        var txt = props.legend;
        if (((_a = props.highlightHover) !== null && _a !== void 0 ? _a : false) && !isNaN(highlight[0]) && !isNaN(highlight[1]))
            txt = txt + " (".concat(moment.utc(highlight[0]).format('MM/DD/YY hh:mm:ss'), ": ").concat(highlight[1].toPrecision(6), ")");
        return React.createElement(LineLegend_1.default, { size: 'sm', label: txt, color: props.color, lineStyle: props.lineStyle, setEnabled: setEnabled, enabled: enabled, hasNoData: data == null });
    }, [props.color, props.lineStyle, enabled, data]);
    var createContextData = React.useCallback(function () {
        return {
            legend: createLegend(),
            axis: props.axis,
            enabled: enabled,
            getMax: function (t) { return (data == null || !enabled ? -Infinity : data.GetLimits(t[0], t[1])[1]); },
            getMin: function (t) { return (data == null || !enabled ? Infinity : data.GetLimits(t[0], t[1])[0]); },
            getPoints: function (t, n) { return (data == null || !enabled ? NaN : data.GetPoints(t, n !== null && n !== void 0 ? n : 1)); }
        };
    }, [props.axis, enabled, dataGuid, createLegend]);
    React.useEffect(function () {
        if (guid === "")
            return;
        context.UpdateData(guid, createContextData());
    }, [createContextData]);
    React.useEffect(function () {
        setDataGuid((0, helper_functions_1.CreateGuid)());
    }, [data]);
    React.useEffect(function () {
        if (data == null || props.data == null || props.data.length === 0 || isNaN(context.XHover))
            setHighlight([NaN, NaN]);
        else {
            try {
                var point = data.GetPoint(context.XHover);
                if (point != null)
                    setHighlight(point);
            }
            catch (_a) {
                setHighlight([NaN, NaN]);
            }
        }
    }, [data, context.XHover]);
    React.useEffect(function () {
        if (props.data == null || props.data.length === 0)
            setData(null);
        else
            setData(new PointNode_1.PointNode(props.data));
    }, [props.data]);
    React.useEffect(function () {
        if (guid === "")
            return;
        context.SetLegend(guid, createLegend());
    }, [highlight, enabled]);
    React.useEffect(function () {
        if (data == null) {
            setVisibleData([]);
            return;
        }
        setVisibleData(data.GetData(context.XDomain[0], context.XDomain[1], true));
    }, [data, context.XDomain[0], context.XDomain[1]]);
    React.useEffect(function () {
        var id = context.AddData(createContextData());
        setGuid(id);
        return function () { context.RemoveData(id); };
    }, []);
    function generateData() {
        var result = "M ";
        if (data == null)
            return "";
        result = result + visibleData.map(function (pt, _) {
            var x = context.XTransformation(pt[0]);
            var y = context.YTransformation(pt[1], GraphContext_1.AxisMap.get(props.axis));
            return "".concat(x, ",").concat(y);
        }).join(" L ");
        return result;
    }
    return (enabled ?
        React.createElement("g", null,
            React.createElement("path", { d: generateData(), style: { fill: 'none', strokeWidth: props.width === undefined ? 3 : props.width, stroke: props.color }, strokeDasharray: GraphContext_1.LineMap.get(props.lineStyle) }),
            showPoints && data != null ? visibleData.map(function (pt, i) { return React.createElement("circle", { key: i, r: 3, cx: context.XTransformation(pt[0]), cy: context.YTransformation(pt[1], GraphContext_1.AxisMap.get(props.axis)), fill: props.color, stroke: 'black', style: { opacity: 0.8 /*, transition: 'cx 0.5s,cy 0.5s'*/ } }); }) : null,
            ((_a = props.highlightHover) !== null && _a !== void 0 ? _a : false) && !isNaN(highlight[0]) && !isNaN(highlight[1]) ?
                React.createElement("circle", { r: 5, cx: context.XTransformation(highlight[0]), cy: context.YTransformation(highlight[1], GraphContext_1.AxisMap.get(props.axis)), fill: props.color, stroke: 'black', style: { opacity: 0.8 /*, transition: 'cx 0.5s,cy 0.5s'*/ } }) : null) : null);
}
exports["default"] = Line;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiTGluZS5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uL3NyYy9MaW5lLnRzeCJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiO0FBQUEseUdBQXlHO0FBQ3pHLG1CQUFtQjtBQUNuQixFQUFFO0FBQ0YscUVBQXFFO0FBQ3JFLEVBQUU7QUFDRix3R0FBd0c7QUFDeEcsd0dBQXdHO0FBQ3hHLHNHQUFzRztBQUN0Ryx3RkFBd0Y7QUFDeEYsRUFBRTtBQUNGLDBDQUEwQztBQUMxQyxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4Ryw0RUFBNEU7QUFDNUUsRUFBRTtBQUNGLDhCQUE4QjtBQUM5Qix3R0FBd0c7QUFDeEcsMEJBQTBCO0FBQzFCLG1EQUFtRDtBQUNuRCxFQUFFO0FBQ0YseUdBQXlHOzs7Ozs7Ozs7Ozs7Ozs7Ozs7QUFHekcsNkJBQStCO0FBQy9CLCtDQUFzRztBQUN0RywrQkFBaUM7QUFDakMseUNBQXNDO0FBQ3RDLDJDQUFzQztBQUN0QyxtRUFBNEQ7QUFlNUQsU0FBUyxJQUFJLENBQUMsS0FBYTs7SUFDdkI7O01BRUU7SUFDSSxJQUFBLEtBQUEsT0FBa0IsS0FBSyxDQUFDLFFBQVEsQ0FBUyxFQUFFLENBQUMsSUFBQSxFQUEzQyxJQUFJLFFBQUEsRUFBRSxPQUFPLFFBQThCLENBQUM7SUFDN0MsSUFBQSxLQUFBLE9BQTBCLEtBQUssQ0FBQyxRQUFRLENBQVMsRUFBRSxDQUFDLElBQUEsRUFBbkQsUUFBUSxRQUFBLEVBQUUsV0FBVyxRQUE4QixDQUFDO0lBQ3JELElBQUEsS0FBQSxPQUE0QixLQUFLLENBQUMsUUFBUSxDQUFtQixDQUFDLEdBQUcsRUFBQyxHQUFHLENBQUMsQ0FBQyxJQUFBLEVBQXRFLFNBQVMsUUFBQSxFQUFFLFlBQVksUUFBK0MsQ0FBQztJQUN4RSxJQUFBLEtBQUEsT0FBd0IsS0FBSyxDQUFDLFFBQVEsQ0FBVSxJQUFJLENBQUMsSUFBQSxFQUFwRCxPQUFPLFFBQUEsRUFBRSxVQUFVLFFBQWlDLENBQUM7SUFD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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/LineLegend.js":
/*!******************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/LineLegend.js ***!
  \******************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

// ******************************************************************************************************
//  LineLegend.tsx - Gbtc
//
//  Copyright © 2023, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  03/04/2023 - C Lackner
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var helper_functions_1 = __webpack_require__(/*! @gpa-gemstone/helper-functions */ "./node_modules/@gpa-gemstone/helper-functions/lib/index.js");
var gpa_symbols_1 = __webpack_require__(/*! @gpa-gemstone/gpa-symbols */ "webpack/sharing/consume/default/@gpa-gemstone/gpa-symbols/@gpa-gemstone/gpa-symbols?6dfb");
var LegendContext_1 = __webpack_require__(/*! ./LegendContext */ "./node_modules/@gpa-gemstone/react-graph/lib/LegendContext.js");
var fontFamily = "-apple-system,BlinkMacSystemFont,\"Segoe UI\",Roboto,\"Helvetica Neue\",Arial,sans-serif,\"Apple Color Emoji\",\"Segoe UI Emoji\",\"Segoe UI Symbol\"";
var nonTextualWidth = 45;
var cssStyle = "margin: auto auto auto 0px; display: inline-block; font-weight: 400; font-family: ".concat(fontFamily, ";");
function LineLegend(props) {
    var _a = __read(React.useState(props.label), 2), label = _a[0], setLabel = _a[1];
    var _b = __read(React.useState(100), 2), legendWidth = _b[0], setLegendWith = _b[1];
    var _c = __read(React.useState(100), 2), legendHeight = _c[0], setLegendHeight = _c[1];
    var _d = __read(React.useState(1), 2), textSize = _d[0], setTextSize = _d[1];
    var _e = __read(React.useState(false), 2), useMultiLine = _e[0], setUseMultiLine = _e[1];
    var _f = __read(React.useState((0, helper_functions_1.CreateGuid)()), 1), guid = _f[0];
    var context = React.useContext(LegendContext_1.LegendContext);
    React.useEffect(function () {
        return function () {
            context.RequestLegendWidth(-1, guid);
        };
    }, []);
    React.useEffect(function () {
        setLabel((props.hasNoData ? gpa_symbols_1.Warning : "") + props.label);
    }, [props.hasNoData, props.label]);
    React.useEffect(function () { return setLegendWith(props.size === 'sm' ? context.SmWidth : context.LgWidth); }, [context.LgWidth, context.SmWidth, props.size]);
    React.useEffect(function () { return setLegendHeight(props.size === 'sm' ? context.SmHeight : context.LgHeight); }, [context.SmHeight, context.LgHeight, props.size]);
    React.useEffect(function () {
        var fontSize = 1;
        var textHeight = (0, helper_functions_1.GetTextHeight)(fontFamily, "".concat(fontSize, "em"), label, "".concat(cssStyle), "".concat(legendWidth - nonTextualWidth, "px"));
        var textWidth = (0, helper_functions_1.GetTextWidth)(fontFamily, "".concat(fontSize, "em"), label, "".concat(cssStyle), "".concat(textHeight, "px"));
        var useML = false;
        context.RequestLegendWidth(textWidth + nonTextualWidth, guid);
        while (fontSize > 0.4 && (textWidth > legendWidth - nonTextualWidth || textHeight > legendHeight)) {
            fontSize = fontSize - 0.05;
            textWidth = (0, helper_functions_1.GetTextWidth)(fontFamily, "".concat(fontSize, "em"), label, "".concat(cssStyle), "".concat(textHeight, "px"), "".concat(useML ? 'normal' : undefined));
            textHeight = (0, helper_functions_1.GetTextHeight)(fontFamily, "".concat(fontSize, "em"), label, "".concat(cssStyle), "".concat(legendWidth - nonTextualWidth, "px"), "".concat(useML ? 'normal' : undefined));
            useML = false;
            // Consider special case when width is limiting but height is available
            if (textWidth > (legendWidth - nonTextualWidth) && textHeight < legendHeight) {
                useML = true;
                textHeight = (0, helper_functions_1.GetTextHeight)(fontFamily, "".concat(fontSize, "em"), label, "".concat(cssStyle), "".concat(legendWidth - nonTextualWidth, "px"), "".concat(useML ? 'normal' : undefined));
                textWidth = legendWidth - nonTextualWidth;
            }
        }
        setTextSize(fontSize);
        setUseMultiLine(useML);
    }, [label, legendWidth, legendHeight, props.size, props.hasNoData]);
    return (React.createElement("div", { style: { height: legendHeight, width: legendWidth } },
        React.createElement("div", { onClick: function () { return props.setEnabled(!props.enabled); }, style: { width: '100%', display: 'flex', alignItems: 'center', marginRight: '5px', height: '100%' } },
            (props.lineStyle === '-' ?
                React.createElement("div", { style: { width: ' 10px', height: 0, borderTop: "2px solid ".concat(props.color), borderRight: "10px solid ".concat(props.color), borderBottom: "2px solid ".concat(props.color), borderLeft: "10px solid ".concat(props.color), overflow: 'hidden', marginRight: '5px', opacity: (props.enabled ? 1 : 0.5) } }) :
                React.createElement("div", { style: { width: ' 10px', height: '4px', borderTop: 'none', borderRight: "3px solid ".concat(props.color), borderBottom: 'none', borderLeft: "3px solid ".concat(props.color), overflow: 'hidden', marginRight: '5px', opacity: (props.enabled ? 1 : 0.5) } })),
            React.createElement("label", { style: { fontFamily: fontFamily, fontWeight: 400, display: 'inline-block', margin: 'auto', marginLeft: 0, fontSize: textSize + 'em', whiteSpace: (useMultiLine ? 'normal' : 'nowrap') } },
                " ",
                label))));
}
exports["default"] = LineLegend;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiTGluZUxlZ2VuZC5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uL3NyYy9MaW5lTGVnZW5kLnRzeCJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiO0FBQUEseUdBQXlHO0FBQ3pHLHlCQUF5QjtBQUN6QixFQUFFO0FBQ0YscUVBQXFFO0FBQ3JFLEVBQUU7QUFDRix3R0FBd0c7QUFDeEcsd0dBQXdHO0FBQ3hHLHNHQUFzRztBQUN0Ryx3RkFBd0Y7QUFDeEYsRUFBRTtBQUNGLDBDQUEwQztBQUMxQyxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4Ryw0RUFBNEU7QUFDNUUsRUFBRTtBQUNGLDhCQUE4QjtBQUM5Qix3R0FBd0c7QUFDeEcsMEJBQTBCO0FBQzFCLG1EQUFtRDtBQUNuRCxFQUFFO0FBQ0YseUdBQXlHOzs7Ozs7Ozs7Ozs7Ozs7Ozs7QUFHekcsNkJBQStCO0FBRS9CLG1FQUEwRjtBQUMxRix5REFBb0Q7QUFDcEQsaURBQXNFO0FBU3RFLElBQU0sVUFBVSxHQUFHLHVKQUE2SSxDQUFBO0FBQ2hLLElBQU0sZUFBZSxHQUFHLEVBQUUsQ0FBQztBQUMzQixJQUFNLFFBQVEsR0FBRyw0RkFBcUYsVUFBVSxNQUFHLENBQUE7QUFFbkgsU0FBUyxVQUFVLENBQUMsS0FBYTtJQUN2QixJQUFBLEtBQUEsT0FBb0IsS0FBSyxDQUFDLFFBQVEsQ0FBUyxLQUFLLENBQUMsS0FBSyxDQUFDLElBQUEsRUFBdEQsS0FBSyxRQUFBLEVBQUUsUUFBUSxRQUF1QyxDQUFDO0lBQ3hELElBQUEsS0FBQSxPQUErQixLQUFLLENBQUMsUUFBUSxDQUFTLEdBQUcsQ0FBQyxJQUFBLEVBQXpELFdBQVcsUUFBQSxFQUFFLGFBQWEsUUFBK0IsQ0FBQztJQUMzRCxJQUFBLEtBQUEsT0FBa0MsS0FBSyxDQUFDLFFBQVEsQ0FBUyxHQUFHLENBQUMsSUFBQSxFQUE1RCxZQUFZLFFBQUEsRUFBRSxlQUFlLFFBQStCLENBQUM7SUFDOUQsSUFBQSxLQUFBLE9BQTBCLEtBQUss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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/LineWithThreshold.js":
/*!*************************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/LineWithThreshold.js ***!
  \*************************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

// ******************************************************************************************************
//  LineWithThreshold.tsx - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  03/24/2021 - C Lackner
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
var moment = __webpack_require__(/*! moment */ "webpack/sharing/consume/default/moment/moment?be5a");
var PointNode_1 = __webpack_require__(/*! ./PointNode */ "./node_modules/@gpa-gemstone/react-graph/lib/PointNode.js");
var helper_functions_1 = __webpack_require__(/*! @gpa-gemstone/helper-functions */ "./node_modules/@gpa-gemstone/helper-functions/lib/index.js");
var LineLegend_1 = __webpack_require__(/*! ./LineLegend */ "./node_modules/@gpa-gemstone/react-graph/lib/LineLegend.js");
function LineWithThreshold(props) {
    var _a;
    /*
      Single Line with ability to turn off and on.
    */
    var _b = __read(React.useState(""), 2), guid = _b[0], setGuid = _b[1];
    var _c = __read(React.useState([NaN, NaN]), 2), highlight = _c[0], setHighlight = _c[1];
    var _d = __read(React.useState(true), 2), enabled = _d[0], setEnabled = _d[1];
    var _e = __read(React.useState(0), 2), wLegend = _e[0], setWLegend = _e[1];
    var _f = __read(React.useState(null), 2), data = _f[0], setData = _f[1];
    var _g = __read(React.useState([NaN, NaN]), 2), threshHoldLimits = _g[0], setThresholdLimits = _g[1];
    var context = React.useContext(GraphContext_1.GraphContext);
    React.useEffect(function () {
        if (guid === "")
            return;
        context.UpdateData(guid, {
            legend: createLegend(),
            getMax: function (t) { return (data == null || !enabled ? -Infinity : Math.max(data.GetLimits(t[0], t[1])[1], threshHoldLimits[1])); },
            getMin: function (t) { return (data == null || !enabled ? Infinity : Math.min(data.GetLimits(t[0], t[1])[0], threshHoldLimits[0])); },
        });
    }, [props, data, enabled]);
    React.useEffect(function () {
        if (props.data.length === 0 || isNaN(context.XHover) || data === null)
            setHighlight([NaN, NaN]);
        else {
            var point = data.GetPoint(context.XHover);
            if (point != null)
                setHighlight(point);
        }
    }, [data, context.XHover]);
    React.useEffect(function () {
        setData(new PointNode_1.PointNode(props.data));
    }, [props.data]);
    React.useEffect(function () {
        if (guid === "")
            return;
        context.SetLegend(guid, createLegend());
    }, [highlight, enabled, wLegend]);
    React.useEffect(function () {
        if (props.legend === undefined)
            return;
        if (props.highlightHover === undefined) {
            setWLegend((0, helper_functions_1.GetTextWidth)("Segoe UI", '1em', props.legend) + 45);
            return;
        }
        var txt = props.legend + " (".concat(moment().format('MM/DD/YY hh:mm:ss'), ": ").concat((-99.999999).toPrecision(8), ")");
        setWLegend((0, helper_functions_1.GetTextWidth)("Segoe UI", '1em', txt) + 45);
    }, [props.legend, props.highlightHover]);
    React.useEffect(function () {
        setGuid(context.AddData({
            legend: createLegend(),
            getMax: function (t) { return (data == null || !enabled ? -Infinity : Math.max(data.GetLimits(t[0], t[1])[1], threshHoldLimits[1])); },
            getMin: function (t) { return (data == null || !enabled ? Infinity : Math.min(data.GetLimits(t[0], t[1])[0], threshHoldLimits[0])); },
        }));
        return function () { context.RemoveData(guid); };
    }, []);
    React.useEffect(function () {
        setThresholdLimits([Math.min.apply(Math, __spreadArray([], __read(props.threshHolds.map(function (t) { return t.Value; })), false)), Math.max.apply(Math, __spreadArray([], __read(props.threshHolds.map(function (t) { return t.Value; })), false))]);
    }, [props.threshHolds]);
    function createLegend() {
        var _a;
        if (props.legend === undefined)
            return undefined;
        var txt = props.legend;
        if (((_a = props.highlightHover) !== null && _a !== void 0 ? _a : false) && !isNaN(highlight[0]) && !isNaN(highlight[1]))
            txt = txt + " (".concat(moment.utc(highlight[0]).format('MM/DD/YY hh:mm:ss'), ": ").concat(highlight[1].toPrecision(6), ")");
        return React.createElement(LineLegend_1.default, { size: 'sm', label: txt, color: props.color, lineStyle: props.lineStyle, setEnabled: setEnabled, enabled: enabled, hasNoData: data == null });
    }
    function generateData() {
        var result = "M ";
        if (data == null)
            return "";
        result = result + data.GetFullData().map(function (pt, _) {
            var x = context.XTransformation(pt[0]);
            var y = context.YTransformation(pt[1], GraphContext_1.AxisMap.get(props.axis));
            return "".concat(x, ",").concat(y);
        }).join(" L ");
        return result;
    }
    return (enabled ?
        React.createElement("g", null,
            React.createElement("path", { d: generateData(), style: { fill: 'none', strokeWidth: 3, stroke: props.color, transition: 'd 0.5s' }, strokeDasharray: GraphContext_1.LineMap.get(props.lineStyle) }),
            data != null ? data.GetFullData().map(function (pt, i) { return React.createElement("circle", { key: i, r: 3, cx: context.XTransformation(pt[0]), cy: context.YTransformation(pt[1], GraphContext_1.AxisMap.get(props.axis)), fill: props.color, stroke: 'black', style: { opacity: 0.8, transition: 'cx 0.5s,cy 0.5s' } }); }) : null,
            ((_a = props.highlightHover) !== null && _a !== void 0 ? _a : false) && !isNaN(highlight[0]) && !isNaN(highlight[1]) ?
                React.createElement("circle", { r: 5, cx: context.XTransformation(highlight[0]), cy: context.YTransformation(highlight[1], GraphContext_1.AxisMap.get(props.axis)), fill: props.color, stroke: 'black', style: { opacity: 0.8, transition: 'cx 0.5s,cy 0.5s' } }) : null,
            props.threshHolds.map(function (t, i) { return React.createElement("path", { key: i, d: "M ".concat(context.XTransformation(context.XDomain[0]), ",").concat(context.YTransformation(t.Value, GraphContext_1.AxisMap.get(props.axis)), " H ").concat(context.XTransformation(context.XDomain[1])), style: { fill: 'none', strokeWidth: 3, stroke: t.Color, transition: 'd 0.5s' }, strokeDasharray: '10,5' }); })) : null);
}
exports["default"] = LineWithThreshold;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiTGluZVdpdGhUaHJlc2hvbGQuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi9zcmMvTGluZVdpdGhUaHJlc2hvbGQudHN4Il0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiI7QUFBQSx5R0FBeUc7QUFDekcsZ0NBQWdDO0FBQ2hDLEVBQUU7QUFDRixxRUFBcUU7QUFDckUsRUFBRTtBQUNGLHdHQUF3RztBQUN4Ryx3R0FBd0c7QUFDeEcsc0dBQXNHO0FBQ3RHLHdGQUF3RjtBQUN4RixFQUFFO0FBQ0YsMENBQTBDO0FBQzFDLEVBQUU7QUFDRix3R0FBd0c7QUFDeEcsd0dBQXdHO0FBQ3hHLDRFQUE0RTtBQUM1RSxFQUFFO0FBQ0YsOEJBQThCO0FBQzlCLHdHQUF3RztBQUN4RywwQkFBMEI7QUFDMUIsbURBQW1EO0FBQ25ELEVBQUU7QUFDRix5R0FBeUc7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7OztBQUd6Ryw2QkFBK0I7QUFFL0IsK0NBQTJGO0FBQzNGLCtCQUFpQztBQUNqQyx5Q0FBc0M7QUFDdEMsbUVBQTREO0FBRTVELDJDQUFzQztBQVl0QyxTQUFTLGlCQUFpQixDQUFDLEtBQWE7O0lBQ3RDOztNQUVFO0lBQ0ksSUFBQSxLQUFBLE9BQWtCLEtBQUssQ0FBQyxRQUFRLENBQVMsRUFBRSxDQUFDLElBQUEsRUFBM0MsSUFBSSxRQUFBLEVBQUUsT0FBTyxRQUE4QixDQUFDO0lBQzdDLElBQUEsS0FBQSxPQUE0QixLQUFLLENBQUMsUUFBUSxDQUFtQixDQUFDLEdBQUcsRUFBQyxHQUFHLENBQUMsQ0FBQyxJQUFBLEVBQXRFLFNBQVMsUUFBQSxFQUFFLFlBQVksUUFBK0MsQ0FBQztJQUN4RSxJQUFBLEtBQUEsT0FBd0IsS0FBSyxDQUFDLFFBQVEsQ0FBVSxJQUFJLENBQUMsSUFBQSxFQUFwRCxPQUFPLFFBQUEsRUFBRSxVQUFVLFFBQWlDLENBQUM7SUFDdEQsSUFBQSxLQUFBLE9BQXdCLEtBQUssQ0FBQyxRQUFRLENBQVMsQ0FBQyxDQUFDLElBQUEsRUFB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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/LogAxis.js":
/*!***************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/LogAxis.js ***!
  \***************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

// ******************************************************************************************************
//  LogAxis.tsx - Gbtc
//
//  Copyright © 2022, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  08/18/2021 - C Lackner
//       Generated original version of source code.
//
//  06/27/2022 - A Hagemeyer
//       Changed to support Logarithmic scale as X Axis
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
var helper_functions_1 = __webpack_require__(/*! @gpa-gemstone/helper-functions */ "./node_modules/@gpa-gemstone/helper-functions/lib/index.js");
function LogAxis(props) {
    var _a, _b, _c, _d;
    /*
       Used on the bottom of the plot
    */
    var context = React.useContext(GraphContext_1.GraphContext);
    var _e = __read(React.useState([]), 2), tick = _e[0], setTick = _e[1];
    var _f = __read(React.useState(0), 2), hLabel = _f[0], setHlabel = _f[1];
    var _g = __read(React.useState(0), 2), hAxis = _g[0], setHAxis = _g[1];
    var _h = __read(React.useState(0), 2), deltaW = _h[0], setDeltaW = _h[1];
    var _j = __read(React.useState(0), 2), steps = _j[0], setSteps = _j[1];
    var _k = __read(React.useState(0), 2), tickStart = _k[0], setTickStart = _k[1];
    React.useEffect(function () {
        if (context.XDomain[0] <= 0) {
            context.XDomain[0] = Math.pow(10, Math.floor(Math.log10(Math.abs(context.XDomain[0])) * -1));
        }
        if (context.XDomain[1] <= 0) {
            context.XDomain[1] = Math.pow(10, (Math.ceil(Math.log10(Math.abs(context.XDomain[1]))) * -1) + 1);
        }
        var WMax = Math.ceil(Math.max(Math.log10(context.XDomain[0]), Math.log10(context.XDomain[1])));
        var WMin = Math.floor(Math.min(Math.log10(context.XDomain[0]), Math.log10(context.XDomain[1])));
        setDeltaW(WMax - WMin);
        setTickStart(WMin);
    }, [context.XDomain]);
    React.useEffect(function () {
        // Steps only change after 300 ms to avoid jumping
        var h = setTimeout(function () {
            if (deltaW < 3)
                setSteps(0.25 * (deltaW / 2));
            else if (deltaW >= 3 && deltaW < 6)
                setSteps(0.5);
            else
                setSteps(Math.floor(deltaW / 4));
        }, 500);
        return function () { clearTimeout(h); };
    }, [deltaW]);
    // Adjusting for x axis label
    React.useEffect(function () {
        var dX = (props.label !== undefined ? (0, helper_functions_1.GetTextHeight)("Segoe UI", "1em", props.label) : 0);
        setHlabel(dX);
    }, [tick, props.label]);
    // Adjusting for x axis tick labels the "..." operator simply grabs array of ticks
    React.useEffect(function () {
        var dX = Math.max.apply(Math, __spreadArray([], __read(tick.map(function (t) { return (0, helper_functions_1.GetTextHeight)("Segoe UI", '1em', t.toString()); })), false));
        dX = (isFinite(dX) ? dX : 0) + 12;
        setHAxis(dX);
    }, [tick]);
    // Resizing if the label and ticks are not the correct height
    React.useEffect(function () {
        if (hAxis + hLabel !== props.heightAxis)
            props.setHeight(hAxis + hLabel);
    }, [hAxis, hLabel, props.heightAxis, props.setHeight]);
    React.useEffect(function () {
        var newTicks;
        if (deltaW === 0 || steps === 0) {
            if (context.XDomain[0] < 0)
                newTicks = [Math.pow(10, Math.floor(Math.log10(Math.abs(context.XDomain[0])) * -1)), Math.pow(10, Math.abs(Math.ceil(Math.log10(context.XDomain[1]))))];
            else
                newTicks = [Math.pow(10, Math.log10(context.XDomain[0]))];
        }
        else {
            newTicks = [Math.pow(10, tickStart)];
            if (deltaW >= 3) { // scale == 1
                for (var i = tickStart + (steps); i <= Math.log10(context.XDomain[1]) + steps; i += (steps)) {
                    if (!Number.isInteger(i) && i > 1 && deltaW > 3) {
                        var lower = Math.floor(Math.pow(10, i) / Math.pow(10, Math.ceil(i))) * Math.pow(10, Math.ceil(i));
                        var upper = Math.ceil(Math.pow(10, i) / Math.pow(10, Math.floor(i))) * Math.pow(10, Math.floor(i));
                        if (Math.abs(upper - Math.pow(10, i)) < Math.abs(lower - Math.pow(10, i)))
                            newTicks.push(upper);
                        else
                            newTicks.push(lower);
                    }
                    else
                        newTicks.push(Math.pow(10, i));
                }
            }
            newTicks = newTicks.filter(function (t) { return t >= context.XDomain[0] && t <= context.XDomain[1]; });
            // guarantee at least 3 ticks
            if (newTicks.length < 3) {
                var c = (Math.log10(context.XDomain[0]) + Math.log10(context.XDomain[1])) * 0.5;
                newTicks = [context.XDomain[0], Math.pow(10, c), context.XDomain[1]];
            }
        }
        // If first Tick is outside visible move it to zero crossing
        setTick(newTicks.map(function (t) { return Math.max(t, context.XDomain[0]); }));
    }, [context.XDomain, deltaW, steps]);
    function getDigits(x) {
        var d;
        if (x >= 1)
            d = 0;
        else if (Math.floor(Math.abs(-Math.log10(x))) > 100)
            d = 100;
        else
            d = Math.abs(Math.floor(Math.log10(x)));
        return d;
    }
    return (React.createElement("g", null,
        React.createElement("path", { stroke: 'black', style: { strokeWidth: 1 }, d: "M ".concat(props.offsetLeft - (((_a = props.showLeftMostTick) !== null && _a !== void 0 ? _a : true) ? 0 : 8), " ").concat(props.height - props.offsetBottom, " H ").concat(props.width - props.offsetRight + (((_b = props.showRightMostTick) !== null && _b !== void 0 ? _b : true) ? 0 : 8)) }),
        ((_c = props.showLeftMostTick) !== null && _c !== void 0 ? _c : true) ? React.createElement("path", { stroke: 'black', style: { strokeWidth: 1 }, d: "M ".concat(props.offsetLeft, " ").concat(props.height - props.offsetBottom, " v ").concat(8) }) : null,
        ((_d = props.showRightMostTick) !== null && _d !== void 0 ? _d : true) ? React.createElement("path", { stroke: 'black', style: { strokeWidth: 1 }, d: "M ".concat(props.width - props.offsetRight, " ").concat(props.height - props.offsetBottom, " v ").concat(8) }) : null,
        props.showTicks === undefined || props.showTicks ?
            React.createElement(React.Fragment, null,
                tick.map(function (l, i) { var _a; return React.createElement("path", { key: (l.toFixed(50)), stroke: 'lightgrey', strokeOpacity: ((_a = props.showGrid) !== null && _a !== void 0 ? _a : false) ? '0.8' : '0.0', style: { strokeWidth: 1, transition: 'd 0.5s' }, d: "M ".concat(context.XTransformation(l), " ").concat(props.height - props.offsetBottom, " V ").concat(props.offsetTop) }); }),
                tick.map(function (l, i) { return React.createElement("path", { key: (l.toFixed(50)), stroke: 'black', style: { strokeWidth: 1, transition: 'd 0.5s' }, d: "M ".concat(context.XTransformation(l), " ").concat(props.height - props.offsetBottom + 6, " v ").concat(-6) }); }),
                tick.map(function (l, i) { return React.createElement("text", { fill: 'black', key: (l.toFixed(50)), style: { fontSize: '1em', textAnchor: 'middle', dominantBaseline: 'hanging', transition: 'x 0.5s, y 0.5s' }, y: props.height - props.offsetBottom + 8, x: context.XTransformation(l) }, (l.toFixed(getDigits(l)))); }))
            : null,
        props.label !== undefined ? React.createElement("text", { fill: 'black', style: { fontSize: '1em', textAnchor: 'middle', dominantBaseline: 'middle' }, x: props.offsetLeft + ((props.width - props.offsetLeft - props.offsetRight) / 2), y: props.height - props.offsetBottom + hAxis }, props.label) : null));
}
exports["default"] = React.memo(LogAxis);
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiTG9nQXhpcy5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uL3NyYy9Mb2dBeGlzLnRzeCJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiO0FBQUEseUdBQXlHO0FBQ3pHLHNCQUFzQjtBQUN0QixFQUFFO0FBQ0YscUVBQXFFO0FBQ3JFLEVBQUU7QUFDRix3R0FBd0c7QUFDeEcsd0dBQXdHO0FBQ3hHLHNHQUFzRztBQUN0Ryx3RkFBd0Y7QUFDeEYsRUFBRTtBQUNGLDBDQUEwQztBQUMxQyxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4Ryw0RUFBNEU7QUFDNUUsRUFBRTtBQUNGLDhCQUE4QjtBQUM5Qix3R0FBd0c7QUFDeEcsMEJBQTBCO0FBQzFCLG1EQUFtRDtBQUNuRCxFQUFFO0FBQ0YsNEJBQTRCO0FBQzVCLHVEQUF1RDtBQUN2RCxFQUFFO0FBQ0YseUdBQXlHOzs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7QUFFekcsNkJBQStCO0FBQy9CLCtDQUE4QztBQUM5QyxtRUFBK0Q7QUFvQi9ELFNBQVMsT0FBTyxDQUFDLEtBQWE7O0lBQzFCOztNQUVFO0lBQ0YsSUFBTSxPQUFPLEdBQUcsS0FBSyxDQUFDLFVBQVUsQ0FBQywyQkFBWSxDQUFDLENBQUM7SUFDekMsSUFBQSxLQUFBLE9BQWtCLEtBQUssQ0FBQyxRQUFRLENBQVcsRUFBRSxDQUFDLElBQUEsRUFBN0MsSUFBSSxRQUFBLEVBQUUsT0FBTyxRQUFnQyxDQUFDO0lBQy9DLElBQUEsS0FBQSxPQUFzQixLQUFLLENBQUMsUUFBUSxDQUFTLENBQUMsQ0FBQyxJQUFBLEVBQTlDLE1BQU0sUUFBQSxFQUFFLFNBQVMsUUFBNkIsQ0FBQztJQUNoRCxJQUFBLEtBQUEsT0FBb0IsS0FBSyxDQUFDLFFBQVEsQ0FBUyxDQUFDLENBQUMsSUFBQSxFQUE1QyxLQUFLLFFBQUEsRUFBRSxRQUFRLFFBQTZCLENBQUM7SUFDOUMsSUFBQSxLQUFBLE9BQXNCLEtBQUssQ0FBQyxRQUFRLENBQVMsQ0FBQyxDQUFDLElBQUEsRUFBOUMsTUFBTSxRQUFBLEVBQUUsU0FBUyxRQUE2QixDQUFDO0lBQ2hELElBQUEsS0FBQSxPQUFvQixLQUFLLENBQUMsUUFBUSxDQUFTLENBQUMsQ0FBQyxJQUFBLEVBQTVDLEtBQUssUUFBQSxFQUFFLFFBQVEsUUFBNkIsQ0FBQztJQUM5QyxJQUFBLEtBQUEsT0FBNEIsS0FBSyxDQUFDLFFBQVEsQ0FBUyxDQUFDLENBQUMsSUFBQSxFQUFwRCxTQUFTLFFBQUEsRUFBRSxZQUFZLFFBQTZCLENBQUM7SUFFNUQsS0FBSyxDQUFDLFNBQVMsQ0FBQztRQUNaLElBQUksT0FBTyxDQUFDLE9BQU8sQ0FBQyxDQUFDLENBQUMsSUFBSSxDQUFDLEVBQUUsQ0FBQztZQUMxQixPQUFPLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQyxHQUFHLElBQUksQ0FBQyxHQUFHLENBQUMsRUFBRSxFQUFFLElBQUksQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLEtBQUssQ0FBQyxJQUFJLENBQUMsR0FBRyxDQUFDLE9BQU8sQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztRQUNqRyxDQUFDO1FBQ0QsSUFBSSxPQUFPLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUMsRUFBRSxDQUFDO1lBQzFCLE9BQU8sQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDLEdBQUcsSUFBSSxDQUFDLEdBQUcsQ0FBQyxFQUFFLEVBQUUsQ0FBQyxJQUFJLENBQUMsSUFBSSxDQUFDLElBQUksQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxPQUFPLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxDQUFDLENBQUM7UUFDdEcsQ0FBQztRQUNELElBQU0sSUFBSSxHQUFHLElBQUksQ0FBQyxJQUFJLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLE9BQU8sQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDLENBQUMsRUFBRSxJQUFJLENBQUMsS0FBSyxDQUFDLE9BQU8sQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7UUFDakcsSUFBTSxJQUFJLEdBQUcsSUFBSSxDQUFDLEtBQUssQ0FBQyxJQUFJLENBQUMsR0FBRyxDQUFDLElBQUksQ0FBQyxLQUFLLENBQUMsT0FBTyxDQUFDLE9BQU8sQ0FBQyxDQUFDLENBQUMsQ0FBQyxFQUFFLElBQUksQ0FBQyxLQUFLLENBQUMsT0FBTyxDQUFDLE9BQU8sQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztRQUNsRyxTQUFTLENBQUMsSUFBSSxHQUFHLElBQUksQ0FBQyxDQUFDO1FBQ3ZCLFlBQVksQ0FBQyxJQUFJLENBQUMsQ0FBQztJQUN2QixDQUFDLEVBQUUsQ0FBQyxPQUFPLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQTtJQUVyQixLQUFLLENBQUMsU0FBUyxDQUFDO1FBQ1osa0RBQWtEO1FBQ2xELElBQU0sQ0FBQyxHQUFHLFVBQVUsQ0FBQztZQUNqQixJQUFJLE1BQU0sR0FBRyxDQUFDO2dCQUNWLFFBQVEsQ0FBQyxJQUFJLEdBQUcsQ0FBQyxNQUFNLEdBQUcsQ0FBQyxDQUFDLENBQUMsQ0FBQztpQkFDN0IsSUFBSSxNQUFNLElBQUksQ0FBQyxJQUFJLE1BQU0sR0FBRyxDQUFDO2dCQUM5QixRQUFRLENBQUMsR0FBRyxDQUFDLENBQUM7O2dCQUVkLFFBQVEsQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLE1BQU0sR0FBRyxDQUFDLENBQUMsQ0FBQyxDQUFBO1FBQ3hDLENBQUMsRUFBRSxHQUFHLENBQUMsQ0FBQTtRQUNQLE9BQU8sY0FBUSxZQUFZLENBQUMsQ0FBQyxDQUFDLENBQUEsQ0FBQyxDQUFDLENBQUE7SUFDcEMsQ0FBQyxFQUFFLENBQUMsTUFBTSxDQUFDLENBQUMsQ0FBQTtJQUdaLDZCQUE2QjtJQUM3QixLQUFLLENBQUMsU0FBUyxDQUFDO1FBQ1osSUFBTSxFQUFFLEdBQUcsQ0FBQyxLQUFLLENBQUMsS0FBSyxLQUFLLFNBQVMsQ0FBQyxDQUFDLENBQUMsSUFBQSxnQ0FBYSxFQUFDLFVBQVUsRUFBRSxLQUFLLEVBQUUsS0FBSyxDQUFDLEtBQUssQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztRQUMzRixTQUFTLENBQUMsRUFBRSxDQUFDLENBQUE7SUFDakIsQ0FBQyxFQUFFLENBQUMsSUFBSSxFQUFFLEtBQUssQ0FBQyxLQUFLLENBQUMsQ0FBQyxDQUFBO0lBRXZCLGtGQUFrRjtJQUNsRixLQUFLLENBQUMsU0FBUyxDQUFDO1FBQ1osSUFBSSxFQUFFLEdBQUcsSUFBSSxDQUFDLEdBQUcsT0FBUixJQUFJLDJCQUFRLElBQUksQ0FBQyxHQUFHLENBQUMsVUFBQSxDQUFDLElBQUksT0FBQSxJQUFBLGdDQUFhLEVBQUMsVUFBVSxFQUFFLEtBQUssRUFBRSxDQUFDLENBQUMsUUFBUSxFQUFFLENBQUMsRUFBOUMsQ0FBOEMsQ0FBQyxVQUFDLENBQUM7UUFDcEYsRUFBRSxHQUFHLENBQUMsUUFBUSxDQUFDLEVBQUUsQ0FBQyxDQUFDLENBQUMsQ0FBQyxFQUFFLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxHQUFHLEVBQUUsQ0FBQztRQUNsQyxRQUFRLENBQUMsRUFBRSxDQUFDLENBQUE7SUFDaEIsQ0FBQyxFQUFFLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQTtJQUVWLDZEQUE2RDtJQUM3RCxLQUFLLENBQUMsU0FBUyxDQUFDO1FBQ1osSUFBSSxLQUFLLEdBQUcsTUFBTSxLQUFLLEtBQUssQ0FBQyxVQUFVO1lBQ25DLEtBQUssQ0FBQyxTQUFTLENBQUMsS0FBSyxHQUFHLE1BQU0sQ0FBQyxDQUFDO0lBQ3hDLENBQUMsRUFBRSxDQUFDLEtBQUssRUFBRSxNQUFNLEVBQUUsS0FBSyxDQUFDLFVBQVUsRUFBRSxLQUFLLENBQUMsU0FBUyxDQUFDLENBQUMsQ0FBQTtJQUV0RCxLQUFLLENBQUMsU0FBUyxDQUFDO1FBRVosSUFBSSxRQUFRLENBQUM7UUFDYixJQUFJLE1BQU0sS0FBSyxDQUFDLElBQUksS0FBSyxLQUFLLENBQUMsRUFBRSxDQUFDO1lBQzlCLElBQUksT0FBTyxDQUFDLE9BQU8sQ0FBQyxDQUFDLENBQUMsR0FBRyxDQUFDO2dCQUN0QixRQUFRLEdBQUcsQ0FBQyxJQUFJLENBQUMsR0FBRyxDQUFDLEVBQUUsRUFBRSxJQUFJLENBQUMsS0FBSyxDQUFDLElBQUksQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxPQUFPLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsR0FBRyxDQUFDLENBQUMsQ0FBQyxDQUFDLEVBQUUsSUFBSSxDQUFDLEdBQUcsQ0FBQyxFQUFFLEVBQUUsSUFBSSxDQUFDLEdBQUcsQ0FBQyxJQUFJLENBQUMsSUFBSSxDQUFDLElBQUksQ0FBQyxLQUFLLENBQUMsT0FBTyxDQUFDLE9BQU8sQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7O2dCQUV4SixRQUFRLEdBQUcsQ0FBQyxJQUFJLENBQUMsR0FBRyxDQUFDLEVBQUUsRUFBRSxJQUFJLENBQUMsS0FBSyxDQUFDLE9BQU8sQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7UUFDbEUsQ0FBQzthQUNJLENBQUM7WUFDRixRQUFRLEdBQUcsQ0FBQyxJQUFJLENBQUMsR0FBRyxDQUFDLEVBQUUsRUFBRSxTQUFTLENBQUMsQ0FBQyxDQUFDO1lBQ3JDLElBQUksTUFBTSxJQUFJLENBQUMsRUFBRSxDQUFDLENBQUMsYUFBYTtnQkFDNUIsS0FBSyxJQUFJLENBQUMsR0FBRyxTQUFTLEdBQUcsQ0FBQyxLQUFLLENBQUMsRUFBRSxDQUFDLElBQUksSUFBSSxDQUFDLEtBQUssQ0FBQyxPQUFPLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQyxDQUFDLEdBQUcsS0FBSyxFQUFFLENBQUMsSUFBSSxDQUFDLEtBQUssQ0FBQyxFQUFFLENBQUM7b0JBQzFGLElBQUksQ0FBQyxNQUFNLENBQUMsU0FBUyxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUMsR0FBRyxDQUFDLElBQUksTUFBTSxHQUFHLENBQUMsRUFBRSxDQUFDO3dCQUM5QyxJQUFNLEtBQUssR0FBRyxJQUFJLENBQUMsS0FBSyxDQUFDLElBQUksQ0FBQyxHQUFHLENBQUMsRUFBRSxFQUFFLENBQUMsQ0FBQyxHQUFHLElBQUksQ0FBQyxHQUFHLENBQUMsRUFBRSxFQUFFLElBQUksQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxHQUFHLElBQUksQ0FBQyxHQUFHLENBQUMsRUFBRSxFQUFFLElBQUksQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQzt3QkFDcEcsSUFBTSxLQUFLLEdBQUcsSUFBSSxDQUFDLElBQUksQ0FBQyxJQUFJLENBQUMsR0FBRyxDQUFDLEVBQUUsRUFBRSxDQUFDLENBQUMsR0FBRyxJQUFJLENBQUMsR0FBRyxDQUFDLEVBQUUsRUFBRSxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsR0FBRyxJQUFJLENBQUMsR0FBRyxDQUFDLEVBQUUsRUFBRSxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7d0JBQ3JHLElBQUksSUFBSSxDQUFDLEdBQUcsQ0FBQyxLQUFLLEdBQUcsSUFBSSxDQUFDLEdBQUcsQ0FBQyxFQUFFLEVBQUUsQ0FBQyxDQUFDLENBQUMsR0FBRyxJQUFJLENBQUMsR0FBRyxDQUFDLEtBQUssR0FBRyxJQUFJLENBQUMsR0FBRyxDQUFDLEVBQUUsRUFBRSxDQUFDLENBQUMsQ0FBQzs0QkFDckUsUUFBUSxDQUFDLElBQUksQ0FBQyxLQUFLLENBQUMsQ0FBQzs7NEJBRXJCLFFBQVEsQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUM7b0JBQzdCLENBQUM7O3dCQUVHLFFBQVEsQ0FBQyxJQUFJLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxFQUFFLEVBQUUsQ0FBQyxDQUFDLENBQUMsQ0FBQztnQkFDdkMsQ0FBQztZQUNMLENBQUM7WUFDRCxR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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/Oval.js":
/*!************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/Oval.js ***!
  \************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

// ******************************************************************************************************
//  Oval.tsx - Gbtc
//
//  Copyright © 2024, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  09/03/2024 - Preston Crawford
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var helper_functions_1 = __webpack_require__(/*! @gpa-gemstone/helper-functions */ "./node_modules/@gpa-gemstone/helper-functions/lib/index.js");
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
var Oval = function (props) {
    var _a;
    var context = React.useContext(GraphContext_1.GraphContext);
    var _b = __read(React.useState(""), 2), guid = _b[0], setGuid = _b[1];
    var _c = __read(React.useState(1), 2), textSize = _c[0], setTextSize = _c[1];
    // Update data series information in the graph context based on circle properties
    React.useEffect(function () {
        if (guid === "")
            return;
        context.UpdateData(guid, {
            axis: props.Axis,
            legend: undefined,
            getMax: function (t) { return (t[0] < props.Data[0] && t[1] > props.Data[1] ? props.Data[2] : undefined); },
            getMin: function (t) { return (t[0] < props.Data[0] && t[1] > props.Data[1] ? props.Data[2] : undefined); },
        });
    }, [props.Axis, props.Data]);
    // Add a new data series on component mount / removing on unmount
    React.useEffect(function () {
        var id = context.AddData({
            axis: props.Axis,
            legend: undefined,
            getMax: function (t) { return (t[0] < props.Data[0] && t[1] > props.Data[1] ? props.Data[2] : undefined); },
            getMin: function (t) { return (t[0] < props.Data[0] && t[1] > props.Data[1] ? props.Data[2] : undefined); },
        });
        setGuid(id);
        return function () {
            context.RemoveData(id);
        };
    }, []);
    // Adjust text size within the oval to ensure it fits
    React.useEffect(function () {
        if (props.Text === undefined)
            return;
        var fontFamily = "Segoe UI";
        var fontSizeUnit = "em";
        var ovalWidth = Math.abs(context.XTransformation(props.Data[1]) - context.XTransformation(props.Data[0])) + (2 * props.Radius);
        var ovalHeight = 2 * props.Radius;
        var minSize = 0.05;
        var maxSize = 5;
        var bestSize = maxSize;
        var calculateTextSize = function (size) {
            var dX = (0, helper_functions_1.GetTextWidth)(fontFamily, size + fontSizeUnit, props.Text);
            var dY = (0, helper_functions_1.GetTextHeight)(fontFamily, size + fontSizeUnit, props.Text);
            return { dX: dX, dY: dY };
        };
        while (maxSize - minSize > 0.01) {
            var midSize = (maxSize + minSize) / 2;
            var _a = calculateTextSize(midSize), dX = _a.dX, dY = _a.dY;
            if (dX <= ovalWidth && dY <= ovalHeight) {
                bestSize = midSize;
                minSize = midSize; // Try larger
            }
            else
                maxSize = midSize; // Try smaller
        }
        setTextSize(bestSize);
    }, [props.Text, props.Radius, context.XTransformation, props.Data]);
    // Set up a click handler if provided in props
    React.useEffect(function () {
        if (guid === "" || props.OnClick === undefined)
            return;
        context.UpdateSelect(guid, { onClick: onClick });
    }, [props.OnClick, context.UpdateFlag]);
    // Handle click events on the oval
    function onClick(xClick, yClick) {
        if (props.OnClick === undefined)
            return;
        // Calculate positions and determine if the click was within the oval bounds
        var axis = GraphContext_1.AxisMap.get(props.Axis);
        var xClickTransformed = context.XTransformation(xClick);
        var yClickTransformed = context.YTransformation(yClick, axis);
        var x1Transformed = context.XTransformation(props.Data[0]) - props.Radius;
        var x2Transformed = context.XTransformation(props.Data[1]) + props.Radius;
        var yTransformed = context.YTransformation(props.Data[2], axis);
        var isWithinHorizontalBounds = xClickTransformed >= x1Transformed && xClickTransformed <= x2Transformed;
        var isWithinVerticalBounds = yClickTransformed >= yTransformed - props.Radius && yClickTransformed <= yTransformed + props.Radius;
        if (isWithinHorizontalBounds && isWithinVerticalBounds)
            props.OnClick(xClick, yClick, { setYDomain: context.SetYDomain, setTDomain: context.SetXDomain });
    }
    // Render null if coordinates are not valid, otherwise render the circle / text
    if (!isFinite(context.XTransformation((props.Data[0], props.Data[1]) / 2)) || !isFinite(context.YTransformation(props.Data[2], GraphContext_1.AxisMap.get(props.Axis))))
        return null;
    return (React.createElement("g", null,
        React.createElement("rect", { x: context.XTransformation(props.Data[0]) - props.Radius, y: context.YTransformation(props.Data[2], GraphContext_1.AxisMap.get(props.Axis)) - props.Radius, width: Math.abs(context.XTransformation(props.Data[1]) - context.XTransformation(props.Data[0])) + (2 * props.Radius), height: 2 * props.Radius, rx: props.Radius, ry: props.Radius, fill: props.Color, opacity: props.Opacity, stroke: props.BorderColor, strokeWidth: props.BorderThickness, onClick: function (e) { return onClick(e.clientX, e.clientY); } }),
        props.Text !== undefined ?
            React.createElement("text", { fill: (_a = props.TextColor) !== null && _a !== void 0 ? _a : 'black', style: { fontSize: textSize + 'em', textAnchor: 'middle', dominantBaseline: 'middle' }, y: context.YTransformation(props.Data[2], GraphContext_1.AxisMap.get(props.Axis)), x: (context.XTransformation(props.Data[0]) + context.XTransformation(props.Data[1])) / 2 }, props.Text) : null));
};
exports["default"] = Oval;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiT3ZhbC5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uL3NyYy9PdmFsLnRzeCJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiO0FBQUEseUdBQXlHO0FBQ3pHLG1CQUFtQjtBQUNuQixFQUFFO0FBQ0YscUVBQXFFO0FBQ3JFLEVBQUU7QUFDRix3R0FBd0c7QUFDeEcsd0dBQXdHO0FBQ3hHLHNHQUFzRztBQUN0Ryx3RkFBd0Y7QUFDeEYsRUFBRTtBQUNGLDBDQUEwQztBQUMxQyxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4Ryw0RUFBNEU7QUFDNUUsRUFBRTtBQUNGLDhCQUE4QjtBQUM5Qix3R0FBd0c7QUFDeEcsaUNBQWlDO0FBQ2pDLG1EQUFtRDtBQUNuRCxFQUFFO0FBQ0YseUdBQXlHOzs7Ozs7Ozs7Ozs7Ozs7Ozs7QUFHekcsbUVBQTZFO0FBQzdFLDZCQUErQjtBQUMvQiwrQ0FBaUg7QUFzRWpILElBQU0sSUFBSSxHQUFHLFVBQUMsS0FBYTs7SUFDdkIsSUFBTSxPQUFPLEdBQUcsS0FBSyxDQUFDLFVBQVUsQ0FBQywyQkFBWSxDQUFDLENBQUM7SUFFekMsSUFBQSxLQUFBLE9BQWtCLEtBQUssQ0FBQyxRQUFRLENBQVMsRUFBRSxDQUFDLElBQUEsRUFBM0MsSUFBSSxRQUFBLEVBQUUsT0FBTyxRQUE4QixDQUFDO0lBQzdDLElBQUEsS0FBQSxPQUEwQixLQUFLLENBQUMsUUFBUSxDQUFTLENBQUMsQ0FBQyxJQUFBLEVBQWxELFFBQVEsUUFBQSxFQUFFLFdBQVcsUUFBNkIsQ0FBQztJQUUxRCxpRkFBaUY7SUFDakYsS0FBSyxDQUFDLFNBQVMsQ0FBQztRQUNaLElBQUksSUFBSSxLQUFLLEVBQUU7WUFDWCxPQUFPO1FBRVgsT0FBTyxDQUFDLFVBQVUsQ0FBQyxJQUFJLEVBQUU7WUFDckIsSUFBSSxFQUFFLEtBQUssQ0FBQyxJQUFJO1lBQ2hCLE1BQU0sRUFBRSxTQUFTO1lBQ2pCLE1BQU0sRUFBRSxVQUFDLENBQUMsSUFBSyxPQUFBLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxHQUFHLEtBQUssQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsQ0FBQyxHQUFHLEtBQUssQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLEtBQUssQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLFNBQVMsQ0FBQyxFQUExRSxDQUEwRTtZQUN6RixNQUFNLEVBQUUsVUFBQyxDQUFDLElBQUssT0FBQSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsR0FBRyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsR0FBRyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxTQUFTLENBQUMsRUFBMUUsQ0FBMEU7U0FDN0UsQ0FBQyxDQUFBO0lBQ3JCLENBQUMsRUFBRSxDQUFDLEtBQUssQ0FBQyxJQUFJLEVBQUUsS0FBSyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUE7SUFFNUIsaUVBQWlFO0lBQ2pFLEtBQUssQ0FBQyxTQUFTLENBQUM7UUFFWixJQUFNLEVBQUUsR0FBRyxPQUFPLENBQUMsT0FBTyxDQUFDO1lBQ3ZCLElBQUksRUFBRSxLQUFLLENBQUMsSUFBSTtZQUNoQixNQUFNLEVBQUUsU0FBUztZQUNqQixNQUFNLEVBQUUsVUFBQyxDQUFDLElBQUssT0FBQSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsR0FBRyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsR0FBRyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxTQUFTLENBQUMsRUFBMUUsQ0FBMEU7WUFDekYsTUFBTSxFQUFFLFVBQUMsQ0FBQyxJQUFLLE9BQUEsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLEdBQUcsS0FBSyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLEdBQUcsS0FBSyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsS0FBSyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsU0FBUyxDQUFDLEVBQTFFLENBQTBFO1NBQzdFLENBQUMsQ0FBQTtRQUNqQixPQUFPLENBQUMsRUFBRSxDQUFDLENBQUE7UUFDWCxPQUFPO1lBQ0gsT0FBTyxDQUFDLFVBQVUsQ0FBQyxFQUFFLENBQUMsQ0FBQTtRQUMxQixDQUFDLENBQUE7SUFDTCxDQUFDLEVBQUUsRUFBRSxDQUFDLENBQUM7SUFFUCxxREFBcUQ7SUFDckQsS0FBSyxDQUFDLFNBQVMsQ0FBQztRQUNaLElBQUksS0FBSyxDQUFDLElBQUksS0FBSyxTQUFTO1lBQUUsT0FBTztRQUVyQyxJQUFNLFVBQVUsR0FBRyxVQUFVLENBQUM7UUFDOUIsSUFBTSxZQUFZLEdBQUcsSUFBSSxDQUFDO1FBRTFCLElBQU0sU0FBUyxHQUFHLElBQUksQ0FBQyxHQUFHLENBQUMsT0FBTyxDQUFDLGVBQWUsQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLEdBQUcsT0FBTyxDQUFDLGVBQWUsQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsR0FBRyxDQUFDLENBQUMsR0FBRyxLQUFLLENBQUMsTUFBTSxDQUFDLENBQUM7UUFDakksSUFBTSxVQUFVLEdBQUcsQ0FBQyxHQUFHLEtBQUssQ0FBQyxNQUFNLENBQUM7UUFFcEMsSUFBSSxPQUFPLEdBQUcsSUFBSSxDQUFDO1FBQ25CLElBQUksT0FBTyxHQUFHLENBQUMsQ0FBQztRQUNoQixJQUFJLFFBQVEsR0FBRyxPQUFPLENBQUM7UUFFdkIsSUFBTSxpQkFBaUIsR0FBRyxVQUFDLElBQVk7WUFDbkMsSUFBTSxFQUFFLEdBQUcsSUFBQSwrQkFBWSxFQUFDLFVBQVUsRUFBRSxJQUFJLEdBQUcsWUFBWSxFQUFFLEtBQUssQ0FBQyxJQUFjLENBQUMsQ0FBQztZQUMvRSxJQUFNLEVBQUUsR0FBRyxJQUFBLGdDQUFhLEVBQUMsVUFBVSxFQUFFLElBQUksR0FBRyxZQUFZLEVBQUUsS0FBSyxDQUFDLElBQWMsQ0FBQyxDQUFDO1lBQ2hGLE9BQU8sRUFBRSxFQUFFLElBQUEsRUFBRSxFQUFFLElBQUEsRUFBRSxDQUFDO1FBQ3RCLENBQUMsQ0FBQTtRQUVELE9BQU8sT0FBTyxHQUFHLE9BQU8sR0FBRyxJQUFJLEVBQUUsQ0FBQztZQUM5QixJQUFNLE9BQU8sR0FBRyxDQUFDLE9BQU8sR0FBRyxPQUFPLENBQUMsR0FBRyxDQUFDLENBQUM7WUFDbEMsSUFBQSxLQUFhLGlCQUFpQixDQUFDLE9BQU8sQ0FBQyxFQUFyQyxFQUFFLFFBQUEsRUFBRSxFQUFFLFFBQStCLENBQUM7WUFFOUMsSUFBSSxFQUFFLElBQUksU0FBUyxJQUFJLEVBQUUsSUFBSSxVQUFVLEVBQUUsQ0FBQztnQkFDdEMsUUFBUSxHQUFHLE9BQU8sQ0FBQztnQkFDbkIsT0FBTyxHQUFHLE9BQU8sQ0FBQyxDQUFDLGFBQWE7WUFDcEMsQ0FBQzs7Z0JBQ0csT0FBTyxHQUFHLE9BQU8sQ0FBQyxDQUFDLGNBQWM7UUFDekMsQ0FBQztRQUVELFdBQVcsQ0FBQyxRQUFRLENBQUMsQ0FBQztJQUMxQixDQUFDLEVBQUUsQ0FBQyxLQUFLLENBQUMsSUFBSSxFQUFFLEtBQUssQ0FBQyxNQUFNLEVBQUUsT0FBTyxDQUFDLGVBQWUsRUFBRSxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQztJQUVwRSw4Q0FBOEM7SUFDOUMsS0FBSyxDQUFDLFNBQVMsQ0FBQztRQUNaLElBQUksSUFBSSxLQUFLLEVBQUUsSUFBSSxLQUFLLENBQUMsT0FBTyxLQUFLLFNBQVM7WUFDMUMsT0FBTztRQUVYLE9BQU8sQ0FBQyxZQUFZLENBQUMsSUFBSSxFQUFFLEVBQUUsT0FBTyxTQUFBLEVBQWUsQ0FBQyxDQUFBO0lBQ3hELENBQUMsRUFBRSxDQUFDLEtBQUssQ0FBQyxPQUFPLEVBQUUsT0FBTyxDQUFDLFVBQVUsQ0FBQyxDQUFDLENBQUE7SUFFdkMsa0NBQWtDO0lBQ2xDLFNBQVMsT0FBTyxDQUFDLE1BQWMsRUFBRSxNQUFjO1FBQzNDLElBQUksS0FBSyxDQUFDLE9BQU8sS0FBSyxTQUFTO1lBQzNCLE9BQU87UUFFWCw0RUFBNEU7UUFDNUUsSUFBTSxJQUFJLEdBQUcsc0JBQU8sQ0FBQyxHQUFHLENBQUMsS0FBSyxDQUFDLElBQUksQ0FBQyxDQUFDO1FBQ3JDLElBQU0saUJBQWlCLEdBQUcsT0FBTyxDQUFDLGVBQWUsQ0FBQyxNQUFNLENBQUMsQ0FBQztRQUMxRCxJQUFNLGlCQUFpQixHQUFHLE9BQU8sQ0FBQyxlQUFlLENBQUMsTUFBTSxFQUFFLElBQUksQ0FBQyxDQUFDO1FBRWhFLElBQU0sYUFBYSxHQUFHLE9BQU8sQ0FBQyxlQUFlLENBQUMsS0FBSyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsQ0FBQyxHQUFHLEtBQUssQ0FBQyxNQUFNLENBQUM7UUFDNUUsSUFBTSxhQUFhLEdBQUcsT0FBTyxDQUFDLGVBQWUsQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLEdBQUcsS0FBSyxDQUFDLE1BQU0sQ0FBQztRQUM1RSxJQUFNLFlBQVksR0FBRyxPQUFPLENBQUMsZUFBZSxDQUFDLEtBQUssQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLEVBQUUsSUFBSSxDQUFDLENBQUM7UUFFbEUsSUFBTSx3QkFBd0IsR0FBRyxpQkFBaUIsSUFBSSxhQUFhLElBQUksaUJBQWlCLElBQUksYUFBYSxDQUFDO1FBQzFHLElBQU0sc0JBQXNCLEdBQUcsaUJBQWlCLElBQUksWUFBWSxHQUFHLEtBQUssQ0FBQyxNQUFNLElBQUksaUJBQWlCLElBQUksWUFBWSxHQUFHLEtBQUssQ0FBQyxNQUFNLENBQUM7UUFFcEksSUFBSSx3QkFBd0IsSUFBSSxzQkFBc0I7WUFDbEQsS0FBSyxDQUFDLE9BQU8sQ0FBQyxNQUFNLEVBQUUsTUFBTSxFQUFFLEVBQUUsVUFBVSxFQUFFLE9BQU8sQ0FBQyxVQUFzRCxFQUFFLFVBQVUsRUFBRSxPQUFPLENBQUMsVUFBb0QsRUFBRSxDQUFDLENBQUM7SUFDaE0sQ0FBQztJQUVELCtFQUErRTtJQUMvRSxJQUFJLENBQUMsUUFBUSxDQUFDLE9BQU8sQ0FBQyxlQUFlLENBQUMsQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxFQUFFLEtBQUssQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsR0FBRyxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUMsUUFBUSxDQUFDLE9BQU8sQ0FBQyxlQUFlLENBQUMsS0FBSyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsRUFBRSxzQkFBTyxDQUFDLEdBQUcsQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQztRQUNwSixPQUFPLElBQUksQ0FBQztJQUVoQixPQUFPLENBQ0g7UUFDSSw4QkFDSSxDQUFDLEVBQUUsT0FBTyxDQUFDLGVBQWUsQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLEdBQUcsS0FBSyxDQUFDLE1BQU0sRUFDeEQsQ0FBQyxFQUFFLE9BQU8sQ0FBQyxlQUFlLENBQUMsS0FBSyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsRUFBRSxzQkFBTyxDQUFDLEdBQUcsQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsR0FBRyxLQUFLLENBQUMsTUFBTSxFQUNqRixLQUFLLEVBQUUsSUFBSSxDQUFDLEdBQUcsQ0FBQyxPQUFPLENBQUMsZUFBZSxDQUFDLEtBQUssQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsR0FBRyxPQUFPLENBQUMsZUFBZSxDQUFDLEtBQUssQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxHQUFHLENBQUMsQ0FBQyxHQUFHLEtBQUssQ0FBQyxNQUFNLENBQUMsRUFDckgsTUFBTSxFQUFFLENBQUMsR0FBRyxLQUFLLENBQUMsTUFBTSxFQUN4QixFQUFFLEVBQUUsS0FBSyxDQUFDLE1BQU0sRUFDaEIsRUFBRSxFQUFFLEtBQUssQ0FBQyxNQUFNLEVBQ2hCLElBQUksRUFBRSxLQUFLLENBQUMsS0FBSyxFQUNqQixPQUFPLEVBQUUsS0FBSyxDQUFDLE9BQU8sRUFDdEIsTUFBTSxFQUFFLEtBQUssQ0FBQyxXQUFXLEVBQ3pCLFdBQVcsRUFBRSxLQUFLLENBQUMsZUFBZSxFQUNsQyxPQUFPLEVBQUUsVUFBQyxDQUFDLElBQUssT0FBQSxPQUFPLENBQUMsQ0FBQyxDQUFDLE9BQU8sRUFBRSxDQUFDLENBQUMsT0FBTyxDQUFDLEVBQTdCLENBQTZCLEdBQy9DO1FBRUQsS0FBSyxDQUFDLElBQUksS0FBSyxTQUFTLENBQUMsQ0FBQztZQUN2Qiw4QkFBTSxJQUFJLEVBQUUsTUFBQSxLQUFLLENBQUMsU0FBUyxtQ0FBSSxPQUFPLEVBQUUsS0FBSyxFQUFFLEVBQUUsUUFBUSxFQUFFLFFBQVEsR0FBRyxJQUFJLEVBQUUsVUFBVSxFQUFFLFFBQVEsRUFBRSxnQkFBZ0IsRUFBRSxRQUFRLEVBQUUsRUFDMUgsQ0FBQyxFQUFFLE9BQU8sQ0FBQyxlQUFlLENBQUMsS0FBSyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsRUFBRSxzQkFBTyxDQUFDLEdBQUcsQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsRUFDbEUsQ0FBQyxFQUFFLENBQUMsT0FBTyxDQUFDLGVBQWUsQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLEdBQUcsT0FBTyxDQUFDLGVBQWUsQ0FBQyxLQUFLLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsR0FBRyxDQUFDLElBQ3ZGLEtBQUssQ0FBQyxJQUFJLENBQ1IsQ0FBQyxDQUFDLENBQUMsSUFBSSxDQUNsQixDQUNQLENBQUM7QUFDTixDQUFDLENBQUE7QUFFRCxrQkFBZSxJQUFJLENBQUMifQ==

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/Plot.js":
/*!************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/Plot.js ***!
  \************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";
/* provided dependency */ var console = __webpack_require__(/*! console-browserify */ "./node_modules/console-browserify/index.js");

// ******************************************************************************************************
//  Plot.tsx - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  03/18/2021 - C Lackner
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};
var __values = (this && this.__values) || function(o) {
    var s = typeof Symbol === "function" && Symbol.iterator, m = s && o[s], i = 0;
    if (m) return m.call(o);
    if (o && typeof o.length === "number") return {
        next: function () {
            if (o && i >= o.length) o = void 0;
            return { value: o && o[i++], done: !o };
        }
    };
    throw new TypeError(s ? "Object is not iterable." : "Symbol.iterator is not defined.");
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var _ = __webpack_require__(/*! lodash */ "webpack/sharing/consume/default/lodash/lodash?552c");
var InteractiveButtons_1 = __webpack_require__(/*! ./InteractiveButtons */ "./node_modules/@gpa-gemstone/react-graph/lib/InteractiveButtons.js");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
var helper_functions_1 = __webpack_require__(/*! @gpa-gemstone/helper-functions */ "./node_modules/@gpa-gemstone/helper-functions/lib/index.js");
var lodash_1 = __webpack_require__(/*! lodash */ "webpack/sharing/consume/default/lodash/lodash?552c");
var TimeAxis_1 = __webpack_require__(/*! ./TimeAxis */ "./node_modules/@gpa-gemstone/react-graph/lib/TimeAxis.js");
var LogAxis_1 = __webpack_require__(/*! ./LogAxis */ "./node_modules/@gpa-gemstone/react-graph/lib/LogAxis.js");
var YValueAxis_1 = __webpack_require__(/*! ./YValueAxis */ "./node_modules/@gpa-gemstone/react-graph/lib/YValueAxis.js");
var Legend_1 = __webpack_require__(/*! ./Legend */ "./node_modules/@gpa-gemstone/react-graph/lib/Legend.js");
var LineWithThreshold_1 = __webpack_require__(/*! ./LineWithThreshold */ "./node_modules/@gpa-gemstone/react-graph/lib/LineWithThreshold.js");
var Line_1 = __webpack_require__(/*! ./Line */ "./node_modules/@gpa-gemstone/react-graph/lib/Line.js");
var Button_1 = __webpack_require__(/*! ./Button */ "./node_modules/@gpa-gemstone/react-graph/lib/Button.js");
var HorizontalMarker_1 = __webpack_require__(/*! ./HorizontalMarker */ "./node_modules/@gpa-gemstone/react-graph/lib/HorizontalMarker.js");
var VerticalMarker_1 = __webpack_require__(/*! ./VerticalMarker */ "./node_modules/@gpa-gemstone/react-graph/lib/VerticalMarker.js");
var SymbolicMarker_1 = __webpack_require__(/*! ./SymbolicMarker */ "./node_modules/@gpa-gemstone/react-graph/lib/SymbolicMarker.js");
var Circle_1 = __webpack_require__(/*! ./Circle */ "./node_modules/@gpa-gemstone/react-graph/lib/Circle.js");
var Oval_1 = __webpack_require__(/*! ./Oval */ "./node_modules/@gpa-gemstone/react-graph/lib/Oval.js");
var AggregatingCircles_1 = __webpack_require__(/*! ./AggregatingCircles */ "./node_modules/@gpa-gemstone/react-graph/lib/AggregatingCircles.js");
var Infobox_1 = __webpack_require__(/*! ./Infobox */ "./node_modules/@gpa-gemstone/react-graph/lib/Infobox.js");
var HeatMapChart_1 = __webpack_require__(/*! ./HeatMapChart */ "./node_modules/@gpa-gemstone/react-graph/lib/HeatMapChart.js");
var _html2canvas = __webpack_require__(/*! html2canvas */ "./node_modules/html2canvas/dist/html2canvas.js");
var HighlightBox_1 = __webpack_require__(/*! ./HighlightBox */ "./node_modules/@gpa-gemstone/react-graph/lib/HighlightBox.js");
var XValueAxis_1 = __webpack_require__(/*! ./XValueAxis */ "./node_modules/@gpa-gemstone/react-graph/lib/XValueAxis.js");
var html2canvas = _html2canvas;
var SvgStyle = {
    fill: 'none',
    userSelect: 'none',
    WebkitTouchCallout: 'none',
    WebkitUserSelect: 'none',
    KhtmlUserSelect: 'none',
    MozUserSelect: 'none',
    msUserSelect: 'none',
    pointerEvents: 'none',
};
var defaultLegendHeight = 50;
var defaultLegendWidth = 100;
var Plot = function (props) {
    var _a, _b, _c, _d;
    // Type correcting functions to convert props into something usable
    var typeCorrect = React.useCallback(function (arg, arrayIndex) {
        if (arg == null)
            return undefined;
        if (!(arg instanceof Object) || !Object.prototype.hasOwnProperty.call(arg, 'length'))
            return (arrayIndex === 0 ? arg : undefined);
        return arg[arrayIndex];
    }, []);
    var typeCorrectDomain = React.useCallback(function (arg) {
        if (arg === undefined || arg.length === 0)
            return [[0, 1], [0, 1]];
        if (typeof (arg[0]) === 'number')
            return [arg, [0, 1]];
        return arg;
    }, []);
    /*
      Actual plot that will handle Axis etc.
    */
    var SVGref = React.useRef(null);
    var handlers = React.useRef(new Map());
    var wheelTimeout = React.useRef({ timeout: undefined, stopScroll: false });
    var heightChange = React.useRef({ timeout: undefined, extraNeeded: 0, captureID: undefined });
    var widthTimeout = React.useRef({ timeout: undefined, requesterMap: new Map() });
    var guid = React.useMemo(function () { return (0, helper_functions_1.CreateGuid)(); }, []);
    var data = React.useRef(new Map());
    var _e = __read(React.useState(""), 2), dataGuid = _e[0], setDataGuid = _e[1];
    var _f = __read(React.useState(props.defaultTdomain), 2), tDomain = _f[0], setTdomain = _f[1];
    var _g = __read(React.useState(0), 2), tOffset = _g[0], setToffset = _g[1];
    var _h = __read(React.useState(1), 2), tScale = _h[0], setTscale = _h[1];
    var _j = __read(React.useState(Array(GraphContext_1.AxisMap.size).fill([0, 0])), 2), yDomain = _j[0], setYdomain = _j[1];
    var _k = __read(React.useState(Array(GraphContext_1.AxisMap.size).fill(0)), 2), yOffset = _k[0], setYoffset = _k[1];
    var _l = __read(React.useState(Array(GraphContext_1.AxisMap.size).fill(1)), 2), yScale = _l[0], setYscale = _l[1];
    // ToDo: This is hardset to two because it's tied to display, 'left' and 'right'
    var _m = __read(React.useState(Array(2).fill(0)), 2), yHasData = _m[0], setYHasData = _m[1];
    var _o = __read(React.useState('none'), 2), mouseMode = _o[0], setMouseMode = _o[1];
    var _p = __read(React.useState((_a = props.defaultMouseMode) !== null && _a !== void 0 ? _a : 'zoom-rectangular'), 2), selectedMode = _p[0], setSelectedMode = _p[1];
    var _q = __read(React.useState(false), 2), mouseIn = _q[0], setMouseIn = _q[1];
    var _r = __read(React.useState([0, 0]), 2), mousePosition = _r[0], setMousePosition = _r[1];
    var _s = __read(React.useState([0, 0]), 2), mousePositionSnap = _s[0], setMousePositionSnap = _s[1];
    var _t = __read(React.useState([0, 0]), 2), mouseClick = _t[0], setMouseClick = _t[1];
    var _u = __read(React.useState("default"), 2), mouseStyle = _u[0], setMouseStyle = _u[1];
    var moveRequested = React.useRef(false);
    var _v = __read(React.useState(false), 2), photoReady = _v[0], setPhotoReady = _v[1];
    var _w = __read(React.useState(10), 2), offsetTop = _w[0], setOffsetTop = _w[1];
    var _x = __read(React.useState(10), 2), offsetBottom = _x[0], setOffsetBottom = _x[1];
    var _y = __read(React.useState(5), 2), offsetLeft = _y[0], setOffsetLeft = _y[1];
    var _z = __read(React.useState(5), 2), offsetRight = _z[0], setOffsetRight = _z[1];
    var _0 = __read(React.useState(0), 2), heightYFactor = _0[0], setHeightYFactor = _0[1];
    var _1 = __read(React.useState(0), 2), heightXLabel = _1[0], setHeightXLabel = _1[1];
    var _2 = __read(React.useState(0), 2), heightLeftYLabel = _2[0], setHeightLeftYLabel = _2[1];
    var _3 = __read(React.useState(0), 2), heightRightYLabel = _3[0], setHeightRightYLabel = _3[1];
    // States for Props to avoid change notification on ref change
    var _4 = __read(React.useState(props.defaultTdomain), 2), defaultTdomain = _4[0], setDefaultTdomain = _4[1];
    var _5 = __read(React.useState(typeCorrectDomain(props.defaultYdomain)), 2), defaultYdomain = _5[0], setDefaultYdomain = _5[1];
    var _6 = __read(React.useState(0), 2), updateFlag = _6[0], setUpdateFlag = _6[1];
    var _7 = __read(React.useState((_b = props.legendHeight) !== null && _b !== void 0 ? _b : defaultLegendHeight), 2), legendHeight = _7[0], setLegendHeight = _7[1];
    var _8 = __read(React.useState((_c = props.legendWidth) !== null && _c !== void 0 ? _c : defaultLegendWidth), 2), legendWidth = _8[0], setLegendWidth = _8[1];
    var _9 = __read(React.useState(props.height), 2), svgHeight = _9[0], setSVGheight = _9[1];
    var _10 = __read(React.useState(props.width), 2), svgWidth = _10[0], setSVGwidth = _10[1];
    var _11 = __read(React.useState(28), 2), menueWidth = _11[0], setMenueWidth = _11[1];
    var applyToYDomain = React.useCallback(function (predicate) {
        var newDomain = __spreadArray([], __read(yDomain), false);
        var apply = false;
        newDomain.forEach(function (d, i, a) {
            // Note: Apply MUST be after predicate, or it short-circuits it
            apply = predicate(d, i, a) || apply;
        });
        if (apply) {
            setYdomain(newDomain);
        }
    }, [yDomain]);
    // Effect to Reset the legend width/height
    React.useEffect(function () {
        if (props.legendHeight !== undefined)
            setLegendHeight(props.legendHeight);
    }, [props.legendHeight]);
    React.useEffect(function () {
        if (props.legendWidth !== undefined)
            setLegendWidth(props.legendWidth);
    }, [props.legendWidth]);
    // Recompute height and width
    React.useEffect(function () {
        setSVGheight(props.height - (props.legend === 'bottom' ? legendHeight : 0));
    }, [props.height, props.legend, legendHeight]);
    React.useEffect(function () {
        setSVGwidth(props.width - (props.legend === 'right' ? legendWidth : 0));
    }, [props.width, props.legend, legendWidth]);
    // enforce T limits
    React.useEffect(function () {
        if (props.Tmin !== undefined && tDomain[0] < props.Tmin)
            setTdomain(function (t) { var _a; return ([(_a = props.Tmin) !== null && _a !== void 0 ? _a : 0, t[1]]); });
        if (props.Tmax !== undefined && tDomain[1] > props.Tmax)
            setTdomain(function (t) { var _a; return ([t[0], (_a = props.Tmax) !== null && _a !== void 0 ? _a : 0]); });
    }, [tDomain]);
    // enforce Y limits
    React.useEffect(function () {
        var mutateDomain = function (domain, axis, allDomains) {
            var hasApplied = false;
            // Need to type correct our arguements
            var propMin = typeCorrect(props.Ymin, axis);
            var propMax = typeCorrect(props.Ymax, axis);
            if (propMin !== undefined && domain[0] < propMin) {
                allDomains[axis] = [propMin, domain[1]];
                hasApplied = true;
            }
            if (propMax !== undefined && domain[1] > propMax) {
                allDomains[axis] = [domain[0], propMax];
                hasApplied = true;
            }
            return hasApplied;
        };
        applyToYDomain(mutateDomain);
    }, [yDomain]);
    React.useEffect(function () {
        if (!(0, lodash_1.isEqual)(defaultTdomain, props.defaultTdomain))
            setDefaultTdomain(props.defaultTdomain);
    }, [props.defaultTdomain]);
    React.useEffect(function () {
        if (!(0, lodash_1.isEqual)(defaultYdomain, props.defaultYdomain))
            setDefaultYdomain(typeCorrectDomain(props.defaultYdomain));
    }, [props.defaultYdomain]);
    React.useEffect(function () {
        setTdomain(defaultTdomain);
    }, [defaultTdomain]);
    React.useEffect(function () {
        setYdomain(defaultYdomain);
    }, [defaultYdomain]);
    // Adjust top and bottom Offset
    React.useEffect(function () {
        var top = heightYFactor + 10;
        var bottom = heightXLabel;
        if (offsetTop !== top)
            setOffsetTop(top);
        if (offsetBottom !== bottom)
            setOffsetBottom(bottom);
    }, [heightXLabel, heightYFactor]);
    // Adjust Left Offset
    React.useEffect(function () {
        var left = heightLeftYLabel + (props.menuLocation === 'left' ? (menueWidth + 2) : 10);
        setOffsetLeft(left);
    }, [heightLeftYLabel, props.menuLocation, menueWidth]);
    // Adjust Right Offset
    React.useEffect(function () {
        var right = heightRightYLabel + ((props.menuLocation === 'right' || props.menuLocation === undefined) ? (menueWidth + 2) : 10);
        setOffsetRight(right);
    }, [heightRightYLabel, props.menuLocation, menueWidth]);
    // Adjust Y domain defaults
    React.useEffect(function () {
        if (props.yDomain !== 'AutoValue' && props.yDomain !== 'HalfAutoValue')
            return;
        var dataReducerFunc = function (result, series, func, axis) {
            // This part of the data may not belong to the axis we care about at the moment
            var dataAxis = GraphContext_1.AxisMap.get(series.axis);
            if (axis === dataAxis) {
                var value = func([Number.MIN_SAFE_INTEGER, Number.MAX_SAFE_INTEGER]);
                if (value !== undefined)
                    result.push(value);
            }
            return result;
        };
        var newDefaultDomain = defaultYdomain.map(function (yDomain, axis) {
            var yMin = Math.min.apply(Math, __spreadArray([], __read(__spreadArray([], __read(data.current.values()), false).reduce(function (result, series) { return dataReducerFunc(result, series, series.getMin, axis); }, [])), false));
            var yMax = Math.max.apply(Math, __spreadArray([], __read(__spreadArray([], __read(data.current.values()), false).reduce(function (result, series) { return dataReducerFunc(result, series, series.getMax, axis); }, [])), false));
            if (!isNaN(yMin) && !isNaN(yMax) && isFinite(yMin) && isFinite(yMax)) {
                if (props.yDomain === 'AutoValue')
                    return [yMin, yMax];
                // If this condition is satisfied, it means our series is mostly positive range
                else if (Math.abs(yMax) >= Math.abs(yMin))
                    return [0, yMax];
                else
                    return [yMin, 0];
            }
            return [0, 1];
        });
        if (!_.isEqual(newDefaultDomain, defaultYdomain))
            setDefaultYdomain(newDefaultDomain);
    }, [dataGuid, props.yDomain]);
    React.useEffect(function () {
        var newHasData = Array(2);
        var hasFunc = function (axis) { return __spreadArray([], __read(data.current.values()), false).some(function (series) { return GraphContext_1.AxisMap.get(axis) === GraphContext_1.AxisMap.get(series.axis); }); };
        newHasData[0] = hasFunc('left');
        newHasData[1] = hasFunc('right');
        setYHasData(newHasData);
    }, [dataGuid]);
    // Adjust x axis
    React.useEffect(function () {
        var dT = tDomain[1] - tDomain[0];
        var dTmin = tDomain[0];
        if (dT === 0)
            return;
        if (props.XAxisType === 'log') {
            dT = Math.log10(tDomain[1]) - Math.log10(tDomain[0]);
            dTmin = Math.log10(tDomain[0]);
        }
        var scale = (svgWidth - offsetLeft - offsetRight) / dT;
        setTscale(scale);
        setToffset(offsetLeft - dTmin * scale);
    }, [tDomain, offsetLeft, offsetRight, props.XAxisType, svgWidth]);
    // Adjust y axis
    React.useEffect(function () {
        var e_1, _a;
        var mutateFunction = function (scaleArray, offsetArray, axis) {
            var dY = yDomain[axis][1] - yDomain[axis][0];
            var scale = (svgHeight - offsetTop - offsetBottom) / (dY === 0 ? 0.00001 : dY);
            scaleArray[axis] = -scale;
            offsetArray[axis] = svgHeight - offsetBottom + yDomain[axis][0] * scale;
        };
        // Update every axis
        var newScale = __spreadArray([], __read(yScale), false);
        var newOffset = __spreadArray([], __read(yOffset), false);
        try {
            for (var _b = __values(GraphContext_1.AxisMap.values()), _c = _b.next(); !_c.done; _c = _b.next()) {
                var axis = _c.value;
                mutateFunction(newScale, newOffset, axis);
            }
        }
        catch (e_1_1) { e_1 = { error: e_1_1 }; }
        finally {
            try {
                if (_c && !_c.done && (_a = _b.return)) _a.call(_b);
            }
            finally { if (e_1) throw e_1.error; }
        }
        setYscale(newScale);
        setYoffset(newOffset);
    }, [yDomain, offsetTop, offsetBottom, svgHeight]);
    React.useEffect(function () { setUpdateFlag(function (x) { return x + 1; }); }, [tScale, tOffset, yScale, yOffset]);
    // Change mouse cursor
    React.useEffect(function () {
        var newCursor;
        if (props.cursorOverride == null) {
            switch (selectedMode) {
                case 'pan':
                    newCursor = 'grab';
                    break;
                case 'select':
                    newCursor = 'pointer';
                    break;
                default:
                    newCursor = 'crosshair';
            }
        }
        else
            newCursor = props.cursorOverride;
        setMouseStyle(newCursor);
    }, [selectedMode, props.cursorOverride]);
    // Stop scrolling while zooming
    React.useEffect(function () {
        var cancelWheel = function (e) { if (wheelTimeout.current.stopScroll)
            e.preventDefault(); };
        document.body.addEventListener('wheel', cancelWheel, { passive: false });
        return function () { return document.body.removeEventListener('wheel', cancelWheel); };
    }, []);
    // Execute Plot Capture and leave photo mode
    React.useEffect(function () {
        // ToDo: We can clean this up some and improve performance using html2canvas options (but the biggest hurdle is the legend, which we don't have a lot of options for...)
        if (!photoReady)
            return;
        // we can't immediately complete the request, since some layout things may still be changing...
        clearTimeout(heightChange.current.timeout);
        // timeout to set if we don't see any more changes within 0.05 seconds
        heightChange.current.timeout = setTimeout(function () {
            var _a;
            var id = (_a = heightChange.current.captureID) !== null && _a !== void 0 ? _a : guid;
            var element = document.getElementById(id);
            if (element == null) {
                console.error("Could not find document element with id ".concat(id));
            }
            else {
                html2canvas(element).then(function (canvas) {
                    document.body.appendChild(canvas);
                    var imageData = canvas.toDataURL("image/png").replace(/^data:image\/png/, "data:application/octet-stream");
                    var anchorElement = document.createElement("a");
                    anchorElement.href = imageData;
                    anchorElement.download = "".concat(id, ".png");
                    document.body.appendChild(anchorElement);
                    anchorElement.click();
                    // Removing children created/cleanup
                    window.URL.revokeObjectURL(imageData);
                    document.body.removeChild(anchorElement);
                    document.body.removeChild(canvas);
                });
            }
            setPhotoReady(false);
            if (props.onCaptureComplete !== undefined)
                props.onCaptureComplete();
        }, 50);
    });
    // requests new legend height/width upto a defined maximum set by props
    var requestLegendHeightChange = React.useCallback(function (newHeight) {
        var _a;
        var heightLimit = props.legend !== 'bottom' ? svgHeight : ((_a = props.legendHeight) !== null && _a !== void 0 ? _a : defaultLegendHeight);
        if (!photoReady)
            heightChange.current.extraNeeded = Math.max(newHeight - heightLimit, 0);
        if (props.legend !== 'bottom')
            return;
        var limitedHeight = Math.min(newHeight, heightLimit);
        if (legendHeight !== limitedHeight)
            setLegendHeight(limitedHeight);
    }, [props.legendHeight, setLegendHeight, legendHeight, props.legend, photoReady]);
    var requestLegendWidthChange = React.useCallback(function (newWidth, requesterID) {
        var _a;
        if (newWidth < 0) {
            widthTimeout.current.requesterMap.delete(requesterID);
            return;
        }
        if (props.legend !== 'right')
            return;
        var limitedWidth = Math.min(newWidth, (_a = props.legendWidth) !== null && _a !== void 0 ? _a : defaultLegendWidth);
        // we can't immediately complete the request, since there are multiple legend items trying to adjust this at a time sometimes
        clearTimeout(widthTimeout.current.timeout);
        widthTimeout.current.requesterMap.set(requesterID, limitedWidth);
        // timeout to set if we don't see any more requests within 0.05 seconds
        widthTimeout.current.timeout = setTimeout(function () {
            var largestRequested = Math.max.apply(Math, __spreadArray([], __read(widthTimeout.current.requesterMap.values()), false));
            if (legendWidth !== largestRequested)
                setLegendWidth(largestRequested);
        }, 50);
    }, [props.legendWidth, setLegendWidth, legendWidth, props.legend]);
    // transforms from pixels into x value. result passed into onClick function 
    var xInvTransform = React.useCallback(function (p) {
        var xT = (p - tOffset) / tScale;
        if (props.XAxisType === 'log')
            xT = Math.pow(10, (p - tOffset) / tScale);
        return xT;
    }, [tOffset, tScale, props.XAxisType]);
    // transforms from pixels into y value. result passed into onClick function
    var yInvTransform = React.useCallback(function (p, a) {
        var axis = (typeof (a) !== 'number') ? GraphContext_1.AxisMap.get(a) : a;
        return (p - yOffset[axis]) / yScale[axis];
    }, [yOffset, yScale]);
    var Reset = React.useCallback(function () {
        setTdomain(defaultTdomain);
        setYdomain(defaultYdomain);
    }, [defaultYdomain, defaultTdomain]);
    // new X transformation from x value into Pixels
    var xTransform = React.useCallback(function (value) {
        var xT = value * tScale + tOffset;
        if (props.XAxisType === 'log') {
            var v = (value === 0 ? tDomain[0] * 0.01 : value);
            xT = Math.log10(v) * tScale + tOffset;
        }
        return xT;
    }, [tScale, tOffset, props.XAxisType, tDomain]);
    // new Y transformation from y value into Pixels
    var yTransform = React.useCallback(function (value, a) {
        var axis = (typeof (a) !== 'number') ? GraphContext_1.AxisMap.get(a) : a;
        return value * yScale[axis] + yOffset[axis];
    }, [yScale, yOffset]);
    // applies offset and contraints to x Pixel value to get something that is plotable
    var xApplyOffset = React.useCallback(function (value) {
        if (value >= 0)
            return Math.min(value + offsetLeft, svgWidth - offsetRight);
        else
            return Math.max(offsetLeft, svgWidth - offsetRight + value);
    }, [offsetLeft, offsetRight, svgWidth]);
    // applies offset and contraints to y Pixel value to get something that is plotable
    var yApplyOffset = React.useCallback(function (value) {
        if (value >= 0)
            return Math.min(value + offsetTop, svgHeight - offsetBottom);
        else
            return Math.max(offsetTop, svgHeight - offsetBottom + value);
    }, [offsetTop, offsetBottom, svgHeight]);
    var setData = React.useCallback(function (key, d) {
        setDataGuid((0, helper_functions_1.CreateGuid)());
        if (d != null)
            data.current.set(key, d);
        else
            data.current.delete(key);
    }, []);
    var addData = React.useCallback(function (d) {
        var key = (0, helper_functions_1.CreateGuid)();
        setData(key, d);
        return key;
    }, []);
    var setLegend = React.useCallback(function (key, legend) {
        var series = data.current.get(key);
        if (series === undefined)
            return;
        series.legend = legend;
        data.current.set(key, series);
    }, []);
    function snapMouseToClosestSeries(pixelPt) {
        var xVal = xInvTransform(pixelPt.x);
        var findClosestPoint = function (result, series) {
            var pointArray = series.getPoints(xVal, 7);
            if (pointArray === undefined)
                return result;
            var ptArrayResult = pointArray.reduce(function (result, pt) {
                var point = [xTransform(pt[0]), yTransform(pt[1], GraphContext_1.AxisMap.get(series.axis))];
                var newDistSq = Math.pow((point[0] - pixelPt.x), 2) + Math.pow((point[1] - pixelPt.y), 2);
                if (result.distSq === undefined || newDistSq < result.distSq)
                    return { pt: { x: point[0], y: point[1] }, distSq: newDistSq };
                return result;
            }, { pt: { x: 0, y: 0 }, distSq: undefined });
            if (ptArrayResult.distSq !== undefined && (result.distSq === undefined || ptArrayResult.distSq < result.distSq))
                return ptArrayResult;
            return result;
        };
        return __spreadArray([], __read(data.current.values()), false).reduce(function (result, series) { return findClosestPoint(result, series); }, { pt: { x: 0, y: 0 }, distSq: undefined }).pt;
    }
    var registerSelect = React.useCallback(function (handler) {
        var key = (0, helper_functions_1.CreateGuid)();
        handlers.current.set(key, handler);
        return key;
    }, []);
    var removeSelect = React.useCallback(function (key) {
        handlers.current.delete(key);
    }, []);
    var updateSelect = React.useCallback(function (key, handler) {
        handlers.current.set(key, handler);
    }, []);
    var setSelection = React.useCallback(function (s) {
        if (s === "reset")
            Reset();
        else if (s === "download") {
            if (props.onDataInspect !== undefined)
                props.onDataInspect(tDomain);
        }
        else if (s === "capture") {
            setPhotoReady(true);
            if (props.onCapture !== undefined)
                heightChange.current.captureID = props.onCapture(heightChange.current.extraNeeded);
        }
        else
            setSelectedMode(s);
    }, [tDomain, Reset, props.onDataInspect]);
    var getConstrainedYDomain = React.useCallback(function (newTDomain) {
        var dataReducerFunc = function (result, series, func, axis) {
            // This part of the data may not belong to the axis we care about at the moment
            var dataAxis = GraphContext_1.AxisMap.get(series.axis);
            if (axis === dataAxis) {
                var value = func(newTDomain);
                if (value !== undefined)
                    result.push(value);
            }
            return result;
        };
        return yDomain.map(function (oldDomain, axis) {
            var yMin = Math.min.apply(Math, __spreadArray([], __read(__spreadArray([], __read(data.current.values()), false).reduce(function (result, series) { return dataReducerFunc(result, series, series.getMin, axis); }, [])), false));
            var yMax = Math.max.apply(Math, __spreadArray([], __read(__spreadArray([], __read(data.current.values()), false).reduce(function (result, series) { return dataReducerFunc(result, series, series.getMax, axis); }, [])), false));
            if (!isNaN(yMin) && !isNaN(yMax) && isFinite(yMin) && isFinite(yMax))
                return [yMin, yMax];
            return yDomain[axis];
        });
    }, [dataGuid, yDomain]);
    function handleMouseWheel(evt) {
        var _a;
        if (props.zoom !== undefined && !props.zoom)
            return;
        if (!selectedMode.includes('zoom'))
            return;
        if (!mouseIn)
            return;
        // while wheel is moving, do not release the lock
        clearTimeout(wheelTimeout.current.timeout);
        wheelTimeout.current.stopScroll = true;
        // flag indicating to lock page scrolling
        wheelTimeout.current.timeout = setTimeout(function () {
            wheelTimeout.current.stopScroll = false;
        }, 200);
        var multiplier = 1.25;
        // event.deltaY positive is wheel down or out and negative is wheel up or in
        if (evt.deltaY < 0)
            multiplier = 0.75;
        if (selectedMode !== 'zoom-horizontal') {
            var x0 = xTransform(tDomain[0]);
            var x1 = xTransform(tDomain[1]);
            if (mousePosition[0] < offsetLeft)
                x1 = multiplier * (x1 - x0) + x0;
            else if (mousePosition[0] > (svgWidth - offsetRight))
                x0 = x1 - multiplier * (x1 - x0);
            else {
                var Xcenter = mousePosition[0];
                x0 = Xcenter - (Xcenter - x0) * multiplier;
                x1 = Xcenter + (x1 - Xcenter) * multiplier;
            }
            if ((x1 - x0) > 10) {
                var newTDomain = void 0;
                if ((_a = props.limitZoom) !== null && _a !== void 0 ? _a : false)
                    newTDomain = [Math.max(defaultTdomain[0], xInvTransform(x0)), Math.min(defaultTdomain[1], xInvTransform(x1))];
                else
                    newTDomain = [xInvTransform(x0), xInvTransform(x1)];
                if (selectedMode === 'zoom-vertical') {
                    var newYDomain = getConstrainedYDomain(newTDomain);
                    if (!_.isEqual(newYDomain, yDomain))
                        setYdomain(newYDomain);
                }
                setTdomain(newTDomain);
            }
        }
        if (selectedMode !== 'zoom-vertical') {
            var newYDomain = yDomain.map(function (domain, axis, allDomains) {
                var _a;
                var y0 = yTransform(domain[0], axis);
                var y1 = yTransform(domain[1], axis);
                if (mousePosition[1] < offsetTop)
                    y1 = multiplier * (y1 - y0) + y0;
                else if (mousePosition[1] > (svgHeight - offsetBottom))
                    y0 = y1 - multiplier * (y1 - y0);
                else {
                    var Ycenter = mousePosition[1];
                    y0 = Ycenter - (Ycenter - y0) * multiplier;
                    y1 = Ycenter + (y1 - Ycenter) * multiplier;
                }
                if (Math.abs(y1 - y0) > 10) {
                    if ((_a = props.limitZoom) !== null && _a !== void 0 ? _a : false)
                        return [Math.max(defaultYdomain[axis][0], yInvTransform(y0, axis)), Math.min(defaultYdomain[axis][1], yInvTransform(y1, axis))];
                    return [yInvTransform(y0, axis), yInvTransform(y1, axis)];
                }
                return domain;
            });
            if (!_.isEqual(newYDomain, yDomain)) {
                // todo: added contraint to t domain when mode is zoom-horizontal
                setYdomain(newYDomain);
            }
        }
    }
    function handleMouseMove(evt) {
        if (!moveRequested.current)
            requestAnimationFrame(function () { return mouseMoveEvent(evt); });
        moveRequested.current = true;
    }
    function mouseMoveEvent(evt) {
        var _a;
        moveRequested.current = false;
        if (SVGref.current == null)
            return;
        var pt = SVGref.current.createSVGPoint();
        pt.x = evt.clientX;
        pt.y = evt.clientY;
        var ptTransform = pt.matrixTransform(SVGref.current.getScreenCTM().inverse());
        if (mouseMode === 'pan') {
            var dP = mousePosition[0] - ptTransform.x;
            var Plower = xTransform(tDomain[0]);
            var Pupper = xTransform(tDomain[1]);
            var Tmin = xInvTransform(Plower + dP);
            var Tmax = xInvTransform(Pupper + dP);
            if ((props.Tmin === undefined || Tmin > props.Tmin) &&
                (props.Tmax === undefined || Tmax < props.Tmax))
                setTdomain([Tmin, Tmax]);
            var zoomYAxis = function (domain, axis, allDomains) {
                var dY = yInvTransform(mousePosition[1], axis) - yInvTransform(ptTransform.y, axis);
                // Need to type correct our arguements
                var propMin = typeCorrect(props.Ymin, axis);
                var propMax = typeCorrect(props.Ymax, axis);
                if ((propMin === undefined || domain[0] + dY > propMin) &&
                    (propMax === undefined || domain[1] + dY < propMax)) {
                    allDomains[axis] = [domain[0] + dY, domain[1] + dY];
                    return true;
                }
                return false;
            };
            applyToYDomain(zoomYAxis);
        }
        setMousePosition([ptTransform.x, ptTransform.y]);
        // Here on mouse is snapped (if neccessary)
        var ptFinal;
        if ((_a = props.snapMouse) !== null && _a !== void 0 ? _a : false)
            ptFinal = snapMouseToClosestSeries(ptTransform);
        else
            ptFinal = ptTransform;
        setMousePositionSnap([ptFinal.x, ptFinal.y]);
        if (handlers.current.size > 0)
            handlers.current.forEach(function (v) { return (v.onMove !== undefined ? v.onMove(xInvTransform(v.allowSnapping ? ptFinal.x : ptTransform.x), yInvTransform(v.allowSnapping ? ptFinal.y : ptTransform.y, v.axis)) : null); });
    }
    function handleMouseDown(evt) {
        var _a;
        if (SVGref.current == null)
            return;
        var pt = SVGref.current.createSVGPoint();
        pt.x = evt.clientX;
        pt.y = evt.clientY;
        var ptTransform = pt.matrixTransform(SVGref.current.getScreenCTM().inverse());
        setMouseClick([ptTransform.x, ptTransform.y]);
        if (selectedMode.includes('zoom') && (props.zoom === undefined || props.zoom))
            setMouseMode(selectedMode);
        if (selectedMode === 'pan' && (props.pan === undefined || props.pan)) {
            setMouseMode('pan');
            setMouseStyle('grabbing');
        }
        // Todo: Review question: can we just use mousePosition and mousePositionSnap here? 
        // Here on mouse is snapped (if neccessary)
        var ptFinal;
        if ((_a = props.snapMouse) !== null && _a !== void 0 ? _a : false)
            ptFinal = snapMouseToClosestSeries(ptTransform);
        else
            ptFinal = ptTransform;
        if (selectedMode === 'select' && props.onSelect !== undefined)
            props.onSelect(xInvTransform(ptFinal.x), __spreadArray([], __read(GraphContext_1.AxisMap.values()), false).map(function (axis) { return yInvTransform(ptFinal.y, axis); }), {
                setTDomain: updateXDomain,
                setYDomain: updateYDomain
            });
        if (handlers.current.size > 0 && selectedMode === 'select')
            handlers.current.forEach(function (v) { return (v.onClick !== undefined ? v.onClick(xInvTransform(v.allowSnapping ? ptFinal.x : ptTransform.x), yInvTransform(v.allowSnapping ? ptFinal.y : ptTransform.y, v.axis)) : null); });
    }
    function handleMouseUp() {
        if (selectedMode === 'pan' && (props.pan === undefined || props.pan))
            setMouseStyle('grab');
        if (mouseMode.includes('zoom')) {
            if ((Math.abs(mousePosition[0] - mouseClick[0]) < 10) && (Math.abs(mousePosition[1] - mouseClick[1]) < 10)) {
                setMouseMode('none');
                return;
            }
            if (mouseMode !== 'zoom-horizontal') {
                var t0 = Math.min(xInvTransform(mousePosition[0]), xInvTransform(mouseClick[0]));
                var t1 = Math.max(xInvTransform(mousePosition[0]), xInvTransform(mouseClick[0]));
                var newTDomain = [Math.max(tDomain[0], t0), Math.min(tDomain[1], t1)];
                if (selectedMode === 'zoom-vertical') {
                    var newYDomain = getConstrainedYDomain(newTDomain);
                    if (!_.isEqual(newYDomain, yDomain))
                        setYdomain(newYDomain);
                }
                setTdomain(newTDomain);
            }
            if (mouseMode !== 'zoom-vertical') {
                var newYDomain = yDomain.map(function (domain, axis, allDomains) {
                    var y0 = Math.min(yInvTransform(mousePosition[1], axis), yInvTransform(mouseClick[1], axis));
                    var y1 = Math.max(yInvTransform(mousePosition[1], axis), yInvTransform(mouseClick[1], axis));
                    return [Math.max(domain[0], y0), Math.min(domain[1], y1)];
                });
                if (!_.isEqual(newYDomain, yDomain)) {
                    // todo: added contraint to t domain when mode is zoom-horizontal
                    setYdomain(newYDomain);
                }
            }
        }
        setMouseMode('none');
        if (handlers.current.size > 0 && selectedMode === 'select')
            handlers.current.forEach(function (v) { return (v.onRelease !== undefined ? v.onRelease(xInvTransform(v.allowSnapping ? mousePositionSnap[0] : mousePosition[0]), yInvTransform(v.allowSnapping ? mousePositionSnap[1] : mousePosition[1], v.axis)) : null); });
    }
    function handleMouseOut(_) {
        setMouseIn(false);
        if (mouseMode === 'pan')
            setMouseMode('none');
        if (handlers.current.size > 0 && selectedMode === 'select')
            handlers.current.forEach(function (v) { return (v.onPlotLeave !== undefined ? v.onPlotLeave(xInvTransform(v.allowSnapping ? mousePositionSnap[0] : mousePosition[0]), yInvTransform(v.allowSnapping ? mousePositionSnap[1] : mousePosition[1], v.axis)) : null); });
    }
    function handleMouseIn(_) {
        setMouseIn(true);
    }
    function updateXDomain(x) {
        if (x[0] === tDomain[0] && x[1] === tDomain[1])
            return;
        if (x[0] < x[1])
            setTdomain(x);
        else
            setTdomain([x[1], x[0]]);
    }
    function updateYDomain(y) {
        var correctFunction = function (domain, axis, allDomains) {
            if (y[axis][0] === domain[0] && y[axis][1] === domain[1])
                return false;
            if (y[0] < y[1])
                allDomains[axis] = y[axis];
            else
                allDomains[axis] = [y[axis][1], y[axis][0]];
            return true;
        };
        applyToYDomain(correctFunction);
    }
    return (React.createElement(GraphContext_1.ContextWrapper, { XDomain: tDomain, MousePosition: mousePosition, MousePositionSnap: mousePositionSnap, YDomain: yDomain, CurrentMode: selectedMode, MouseIn: mouseIn, UpdateFlag: updateFlag, Data: data, DataGuid: dataGuid, XApplyPixelOffset: xApplyOffset, YApplyPixelOffset: yApplyOffset, XTransform: xTransform, YTransform: yTransform, XInvTransform: xInvTransform, YInvTransform: yInvTransform, SetXDomain: updateXDomain, SetYDomain: updateYDomain, AddData: addData, RemoveData: setData, UpdateData: setData, SetLegend: setLegend, RegisterSelect: registerSelect, RemoveSelect: removeSelect, UpdateSelect: updateSelect },
        React.createElement("div", { id: guid, style: { height: props.height, width: props.width, position: 'relative' } },
            React.createElement("div", { style: { height: svgHeight, width: svgWidth, position: 'absolute', cursor: mouseStyle }, onWheel: handleMouseWheel, onMouseMove: handleMouseMove, onMouseDown: handleMouseDown, onMouseUp: handleMouseUp, onMouseLeave: handleMouseOut, onMouseEnter: handleMouseIn },
                React.createElement("svg", { ref: SVGref, width: svgWidth < 0 ? 0 : svgWidth, height: svgHeight < 0 ? 0 : svgHeight, style: SvgStyle, viewBox: "0 0 ".concat(svgWidth < 0 ? 0 : svgWidth, " ").concat(svgHeight < 0 ? 0 : svgHeight) },
                    props.showBorder !== undefined && props.showBorder ? React.createElement("path", { stroke: 'black', d: "M ".concat(offsetLeft, " ").concat(offsetTop, " H ").concat(svgWidth - offsetRight, " V ").concat(svgHeight - offsetBottom, " H ").concat(offsetLeft, " Z") }) : null,
                    props.XAxisType === 'time' || props.XAxisType === undefined ?
                        React.createElement(TimeAxis_1.default, { label: props.Tlabel, offsetBottom: offsetBottom, offsetLeft: offsetLeft, offsetRight: offsetRight, width: svgWidth, height: svgHeight, setHeight: setHeightXLabel, heightAxis: heightXLabel, showLeftMostTick: !yHasData[0], showRightMostTick: !yHasData[1], showDate: props.showDateOnTimeAxis }) :
                        props.XAxisType === 'value' ? React.createElement(XValueAxis_1.default, { offsetBottom: offsetBottom, offsetLeft: offsetLeft, offsetRight: offsetRight, width: svgWidth, height: svgHeight, setHeight: setHeightXLabel, heightAxis: heightXLabel, label: props.Tlabel, showLeftMostTick: !yHasData[0], showRightMostTick: !yHasData[1] }) :
                            React.createElement(LogAxis_1.default, { offsetTop: offsetTop, showGrid: props.showGrid, label: props.Tlabel, offsetBottom: offsetBottom, offsetLeft: offsetLeft, offsetRight: offsetRight, width: svgWidth, height: svgHeight, setHeight: setHeightXLabel, heightAxis: heightXLabel, showLeftMostTick: !yHasData[0], showRightMostTick: !yHasData[1] }),
                    ((_d = props.hideYAxis) !== null && _d !== void 0 ? _d : false) ? null : (React.createElement(React.Fragment, null,
                        yHasData[0] ? (React.createElement(YValueAxis_1.default, { offsetRight: offsetRight, showGrid: props.showGrid, label: typeCorrect(props.Ylabel, 0), offsetTop: offsetTop, offsetLeft: offsetLeft, offsetBottom: offsetBottom, width: svgWidth, height: svgHeight, setWidthAxis: setHeightLeftYLabel, setHeightFactor: setHeightYFactor, axis: 'left', hAxis: heightLeftYLabel, hFactor: heightYFactor, useFactor: props.useMetricFactors === undefined ? true : props.useMetricFactors })) : null,
                        yHasData[1] ? (React.createElement(YValueAxis_1.default, { offsetRight: offsetRight, showGrid: props.showGrid, label: typeCorrect(props.Ylabel, 1), offsetTop: offsetTop, offsetLeft: offsetLeft, offsetBottom: offsetBottom, width: svgWidth, height: svgHeight, setWidthAxis: setHeightRightYLabel, setHeightFactor: setHeightYFactor, axis: 'right', hAxis: heightRightYLabel, hFactor: heightYFactor, useFactor: props.useMetricFactors === undefined ? true : props.useMetricFactors })) : null)),
                    React.createElement("defs", null,
                        React.createElement("clipPath", { id: "cp-" + guid },
                            React.createElement("path", { stroke: 'none', fill: 'none', d: " M ".concat(offsetLeft, ",").concat(offsetTop - 5, " H  ").concat(svgWidth - offsetRight + 5, " V ").concat(svgHeight - offsetBottom, " H ").concat(offsetLeft, " Z") }))),
                    React.createElement("g", { clipPath: 'url(#cp-' + guid + ')' },
                        React.Children.map(props.children, function (element) {
                            if (!React.isValidElement(element))
                                return null;
                            if (element.type === Line_1.default || element.type === LineWithThreshold_1.default || element.type === Infobox_1.default ||
                                element.type === HorizontalMarker_1.default || element.type === VerticalMarker_1.default || element.type === SymbolicMarker_1.default
                                || element.type === Circle_1.default || element.type === AggregatingCircles_1.default || element.type === HeatMapChart_1.default ||
                                element.type === Oval_1.default || element.type === HighlightBox_1.default)
                                return element;
                            return null;
                        }),
                        !photoReady && (props.showMouse === undefined || (props.showMouse !== 'none' && props.showMouse !== false)) ?
                            React.createElement("path", { stroke: 'black', style: { strokeWidth: 2, opacity: mouseIn ? 0.8 : 0.0 }, d: (props.showMouse !== 'horizontal' ?
                                    "M ".concat(mousePosition[0], " ").concat(offsetTop, " V ").concat(svgHeight - offsetBottom) :
                                    "M ".concat(offsetLeft, " ").concat(mousePosition[1], " H ").concat(svgWidth - offsetRight)) })
                            : null,
                        (props.zoom === undefined || props.zoom) && mouseMode.includes('zoom') ?
                            React.createElement("rect", { fillOpacity: 0.8, fill: 'black', x: mouseMode !== 'zoom-horizontal' ? Math.min(mouseClick[0], mousePosition[0]) : offsetLeft, y: mouseMode !== 'zoom-vertical' ? Math.min(mouseClick[1], mousePosition[1]) : offsetTop, width: mouseMode !== 'zoom-horizontal' ? Math.abs(mouseClick[0] - mousePosition[0]) : (svgWidth - offsetLeft - offsetRight), height: mouseMode !== 'zoom-vertical' ? Math.abs(mouseClick[1] - mousePosition[1]) : (svgHeight - offsetTop - offsetBottom) })
                            : null),
                    (photoReady || props.menuLocation === 'hide') ? React.createElement(React.Fragment, null) :
                        React.createElement(InteractiveButtons_1.default, { showPan: (props.pan === undefined || props.pan), showZoom: props.zoom === undefined || props.zoom, showReset: !(props.pan !== undefined && props.zoom !== undefined && !props.zoom && !props.pan), showSelect: props.onSelect !== undefined || handlers.current.size > 0, showDownload: props.onDataInspect !== undefined, showCapture: props.onCapture !== undefined, currentSelection: selectedMode, setSelection: setSelection, holdOpen: props.holdMenuOpen, heightAvaliable: svgHeight - 22, setWidth: setMenueWidth, x: (props.menuLocation === 'left' ? 14 : (svgWidth - 14 - menueWidth + 20)), y: 22, "data-html2canvas-ignore": "true" }, React.Children.map(props.children, function (element) {
                            if (!React.isValidElement(element))
                                return null;
                            if (element.type === Button_1.default)
                                return element;
                            return null;
                        })))),
            props.legend !== undefined && props.legend !== 'hidden' ?
                React.createElement(Legend_1.default, { location: props.legend, height: legendHeight, width: legendWidth, graphWidth: svgWidth, graphHeight: svgHeight, RequestLegendWidth: requestLegendWidthChange, RequestLegendHeight: requestLegendHeightChange, HideDisabled: photoReady })
                : null)));
};
exports["default"] = Plot;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiUGxvdC5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uL3NyYy9QbG90LnRzeCJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiO0FBQUEseUdBQXlHO0FBQ3pHLG1CQUFtQjtBQUNuQixFQUFFO0FBQ0YscUVBQXFFO0FBQ3JFLEVBQUU7QUFDRix3R0FBd0c7QUFDeEcsd0dBQXdHO0FBQ3hHLHNHQUFzRztBQUN0Ryx3RkFBd0Y7QUFDeEYsRUFBRTtBQUNGLDBDQUEwQztBQUMxQyxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4Ryw0RUFBNEU7QUFDNUUsRUFBRTtBQUNGLDhCQUE4QjtBQUM5Qix3R0FBd0c7QUFDeEcsMEJBQTBCO0FBQzFCLG1EQUFtRDtBQUNuRCxFQUFFO0FBQ0YseUdBQXlHOzs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7OztBQUd6Ryw2QkFBK0I7QUFDL0IsMEJBQTRCO0FBQzVCLDJEQUFzRDtBQUN0RCwrQ0FBNkg7QUFDN0gsbUVBQTBEO0FBQzFELGlDQUErQjtBQUMvQix1Q0FBa0M7QUFDbEMscUNBQWdDO0FBQ2hDLDJDQUFzQztBQUN0QyxtQ0FBOEI7QUFDOUIseURBQW9EO0FBQ3BELCtCQUEwQjtBQUMxQixtQ0FBOEI7QUFDOUIsdURBQWtEO0FBQ2xELG1EQUE4QztBQUM5QyxtREFBOEM7QUFDOUMsbUNBQThCO0FBQzlCLCtCQUF5QjtBQUN6QiwyREFBc0Q7QUFDdEQscUNBQWdDO0FBQ2hDLCtDQUEwQztBQUMxQywwQ0FBNEM7QUFDNUMsK0NBQTBDO0FBQzFDLDJDQUFzQztBQUN0QyxJQUFNLFdBQVcsR0FBUSxZQUFZLENBQUM7QUEyQ3RDLElBQU0sUUFBUSxHQUF3QjtJQUNsQyxJQUFJLEVBQUUsTUFBTTtJQUNaLFVBQVUsRUFBRSxNQUFNO0lBQ2xCLGtCQUFrQixFQUFFLE1BQU07SUFDMUIsZ0JBQWdCLEVBQUUsTUFBTTtJQUN4QixlQUFlLEVBQUUsTUFBTTtJQUN2QixhQUFhLEVBQUUsTUFBTTtJQUNyQixZQUFZLEVBQUUsTUFBTTtJQUNwQixhQUFhLEVBQUUsTUFBTTtDQUN4QixDQUFDO0FBRUYsSUFBTSxtQkFBbUIsR0FBRyxFQUFFLENBQUM7QUFDL0IsSUFBTSxrQkFBa0IsR0FBRyxHQUFHLENBQUM7QUFFL0IsSUFBTSxJQUFJLEdBQW9DLFVBQUMsS0FBSzs7SUFDaEQsbUVBQW1FO0lBQ25FLElBQU0sV0FBVyxHQUFxRSxLQUFLLENBQUMsV0FBVyxDQUFDLFVBQUMsR0FBRyxFQUFFLFVBQVU7UUFDdEgsSUFBSSxHQUFHLElBQUksSUFBSTtZQUFFLE9BQU8sU0FBUyxDQUFDO1FBQ2xDLElBQUksQ0FBQyxDQUFDLEdBQUcsWUFBWSxNQUFNLENBQUMsSUFBSSxDQUFDLE1BQU0sQ0FBQyxTQUFTLENBQUMsY0FBYyxDQUFDLElBQUksQ0FBQyxHQUFHLEVBQUUsUUFBUSxDQUFDO1lBQUUsT0FBTyxDQUFDLFVBQVUsS0FBSyxDQUFDLENBQUMsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxDQUFDLENBQUMsU0FBUyxDQUFDLENBQUM7UUFDbEksT0FBUSxHQUFXLENBQUMsVUFBVSxDQUFDLENBQUM7SUFDbEMsQ0FBQyxFQUFFLEVBQUUsQ0FBQyxDQUFDO0lBQ1AsSUFBTSxpQkFBaUIsR0FBRyxLQUFLLENBQUMsV0FBVyxDQUFDLFVBQUMsR0FBc0Q7UUFDakcsSUFBSSxHQUFHLEtBQUssU0FBUyxJQUFJLEdBQUcsQ0FBQyxNQUFNLEtBQUssQ0FBQztZQUFFLE9BQU8sQ0FBQyxDQUFDLENBQUMsRUFBQyxDQUFDLENBQUMsRUFBRSxDQUFDLENBQUMsRUFBQyxDQUFDLENBQUMsQ0FBQyxDQUFDO1FBQ2pFLElBQUksT0FBTSxDQUFDLEdBQUcsQ0FBQyxDQUFDLENBQUMsQ0FBQyxLQUFLLFFBQVE7WUFBRSxPQUFPLENBQUMsR0FBRyxFQUFDLENBQUMsQ0FBQyxFQUFDLENBQUMsQ0FBQyxDQUF1QixDQUFDO1FBQzFFLE9BQVEsR0FBMEIsQ0FBQztJQUNyQyxDQUFDLEVBQUUsRUFBRSxDQUFDLENBQUM7SUFDUDs7TUFFRTtJQUNGLElBQU0sTUFBTSxHQUFHLEtBQUssQ0FBQyxNQUFNLENBQU0sSUFBSSxDQUFDLENBQUM7SUFDdkMsSUFBTSxRQUFRLEdBQUcsS0FBSyxDQUFDLE1BQU0sQ0FBd0IsSUFBSSxHQUFHLEVBQXFCLENBQUMsQ0FBQztJQUNuRixJQUFNLFlBQVksR0FBRyxLQUFLLENBQUMsTUFBTSxDQUFrRCxFQUFDLE9BQU8sRUFBRSxTQUFTLEVBQUUsVUFBVSxFQUFFLEtBQUssRUFBQyxDQUFDLENBQUM7SUFDNUgsSUFBTSxZQUFZLEdBQUcsS0FBSyxDQUFDLE1BQU0sQ0FDL0IsRUFBQyxPQUFPLEVBQUUsU0FBUyxFQUFFLFdBQVcsRUFBRSxDQUFDLEVBQUUsU0FBUyxFQUFFLFNBQVMsRUFBQyxDQUFDLENBQUM7SUFDOUQsSUFBTSxZQUFZLEdBQUcsS0FBSyxDQUFDLE1BQU0sQ0FBK0QsRUFBQyxPQUFPLEVBQUUsU0FBUyxFQUFFLFlBQVksRUFBRSxJQUFJLEdBQUcsRUFBaUIsRUFBQyxDQUFDLENBQUM7SUFFOUosSUFBTSxJQUFJLEdBQUcsS0FBSyxDQUFDLE9BQU8sQ0FBQyxjQUFNLE9BQUEsSUFBQSw2QkFBVSxHQUFFLEVBQVosQ0FBWSxFQUFFLEVBQUUsQ0FBQyxDQUFDO0lBRW5ELElBQU0sSUFBSSxHQUFHLEtBQUssQ0FBQyxNQUFNLENBQTJCLElBQUksR0FBRyxFQUF1QixDQUFDLENBQUE7SUFDN0UsSUFBQSxLQUFBLE9BQTBCLEtBQUssQ0FBQyxRQUFRLENBQVMsRUFBRSxDQUFDLElBQUEsRUFBbkQsUUFBUSxRQUFBLEVBQUUsV0FBVyxRQUE4QixDQUFDO0lBRXJELElBQUEsS0FBQSxPQUF3QixLQUFLLENBQUMsUUFBUSxDQUFrQixLQUFLLENBQUMsY0FBYyxDQUFDLElBQUEsRUFBNUUsT0FBTyxRQUFBLEVBQUUsVUFBVSxRQUF5RCxDQUFDO0lBQzlFLElBQUEsS0FBQSxPQUF3QixLQUFLLENBQUMsUUFBUSxDQUFTLENBQUMsQ0FBQyxJQUFBLEVBQWhELE9BQU8sUUFBQSxFQUFFLFVBQVUsUUFBNkIsQ0FBQztJQUNsRCxJQUFBLEtBQUEsT0FBc0IsS0FBSyxDQUFDLFFBQVEsQ0FBUyxDQUFDLENBQUMsSUFBQSxFQUE5QyxNQUFNLFFBQUEsRUFBRSxTQUFTLFFBQTZCLENBQUM7SUFFaEQsSUFBQSxLQUFBLE9BQXdCLEtBQUssQ0FBQyxRQUFRLENBQW9CLEtBQUssQ0FBQyxzQkFBTyxDQUFDLElBQUksQ0FBQyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsRUFBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLElBQUEsRUFBekYsT0FBTyxRQUFBLEVBQUUsVUFBVSxRQUFzRSxDQUFDO0lBQzNGLElBQUEsS0FBQSxPQUF3QixLQUFLLENBQUMsUUFBUSxDQUFXLEtBQUssQ0FBQyxzQkFBTyxDQUFDLElBQUksQ0FBQyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsQ0FBQyxJQUFBLEVBQTVFLE9BQU8sUUFBQSxFQUFFLFVBQVUsUUFBeUQsQ0FBQztJQUM5RSxJQUFBLEtBQUEsT0FBc0IsS0FBSyxDQUFDLFFBQVEsQ0FBVyxLQUFLLENBQUMsc0JBQU8sQ0FBQyxJQUFJLENBQUMsQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsSUFBQSxFQUExRSxNQUFNLFFBQUEsRUFBRSxTQUFTLFFBQXlELENBQUM7SUFDbEYsZ0ZBQWdGO0lBQzFFLElBQUEsS0FBQSxPQUEwQixLQUFLLENBQUMsUUFBUSxDQUFZLEtBQUssQ0FBQyxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsSUFBQSxFQUFwRSxRQUFRLFFBQUEsRUFBRSxXQUFXLFFBQStDLENBQUM7SUFFdEUsSUFBQSxLQUFBLE9BQTRCLEtBQUssQ0FBQyxRQUFRLENBQXNCLE1BQU0sQ0FBQyxJQUFBLEVBQXRFLFNBQVMsUUFBQSxFQUFFLFlBQVksUUFBK0MsQ0FBQztJQUN4RSxJQUFBLEtBQUEsT0FBa0MsS0FBSyxDQUFDLFFBQVEsQ0FBYSxNQUFBLEtBQUssQ0FBQyxnQkFBZ0IsbUNBQUksa0JBQWtCLENBQUMsSUFBQSxFQUF6RyxZQUFZLFFBQUEsRUFBRSxlQUFlLFFBQTRFLENBQUM7SUFFM0csSUFBQSxLQUFBLE9BQXdCLEtBQUssQ0FBQyxRQUFRLENBQVUsS0FBSyxDQUFDLElBQUEsRUFBckQsT0FBTyxRQUFBLEVBQUUsVUFBVSxRQUFrQyxDQUFDO0lBQ3ZELElBQUEsS0FBQSxPQUFvQyxLQUFLLENBQUMsUUFBUSxDQUFtQixDQUFDLENBQUMsRUFBRSxDQUFDLENBQUMsQ0FBQyxJQUFBLEVBQTNFLGFBQWEsUUFBQSxFQUFFLGdCQUFnQixRQUE0QyxDQUFDO0lBQzdFLElBQUEsS0FBQSxPQUE0QyxLQUFLLENBQUMsUUFBUSxDQUFtQixDQUFDLENBQUMsRUFBRSxDQUFDLENBQUMsQ0FBQyxJQUFBLEVBQW5GLGlCQUFpQixRQUFBLEVBQUUsb0JBQW9CLFFBQTRDLENBQUM7SUFDckYsSUFBQSxLQUFBLE9BQThCLEtBQUssQ0FBQyxRQUFRLENBQW1CLENBQUMsQ0FBQyxFQUFFLENBQUMsQ0FBQyxDQUFDLElBQUEsRUFBckUsVUFBVSxRQUFBLEVBQUUsYUFBYSxRQUE0QyxDQUFDO0lBQ3ZFLElBQUEsS0FBQSxPQUE4QixLQUFLLENBQUMsUUFBUSxDQUFTLFNBQVMsQ0FBQyxJQUFBLEVBQTlELFVBQVUsUUFBQSxFQUFFLGFBQWEsUUFBcUMsQ0FBQztJQUN0RSxJQUFNLGFBQWEsR0FBRyxLQUFLLENBQUMsTUFBTSxDQUFVLEtBQUssQ0FBQyxDQUFDO0lBRTdDLElBQUEsS0FBQSxPQUE4QixLQUFLLENBQUMsUUFBUSxDQUFVLEtBQUssQ0FBQyxJQUFBLEVBQTNELFVBQVUsUUFBQSxFQUFFLGFBQWEsUUFBa0MsQ0FBQztJQUU3RCxJQUFBLEtBQUEsT0FBNEIsS0FBSyxDQUFDLFFBQVEsQ0FBUyxFQUFFLENBQUMsSUFBQSxFQUFyRCxTQUFTLFFBQUEsRUFBRSxZQUFZLFFBQThCLENBQUM7SUFDdkQsSUFBQSxLQUFBLE9BQWtDLEtBQUssQ0FBQyxRQUFRLENBQVMsRUFBRSxDQUFDLElBQUEsRUFBM0QsWUFBWSxRQUFBLEVBQUUsZUFBZSxRQUE4QixDQUFDO0lBQzdELElBQUEsS0FBQSxPQUE4QixLQUFLLENBQUMsUUFBUSxDQUFTLENBQUMsQ0FBQyxJQUFBLEVBQXRELFVBQVUsUUFBQSxFQUFFLGFBQWEsUUFBNkIsQ0FBQztJQUN4RCxJQUFBLEtBQUEsT0FBZ0MsS0FBSyxDQUFDLFFBQVEsQ0FBUyxDQUFDLENBQUMsSUFBQSxFQUF4RCxXQUFXLFFBQUEsRUFBRSxjQUFjLFFBQTZCLENBQUM7SUFFMUQsSUFBQSxLQUFBLE9BQW9DLEtBQUssQ0FBQyxRQUFRLENBQVMsQ0FBQyxDQUFDLElBQUEsRUFBNUQsYUFBYSxRQUFBLEVBQUUsZ0JBQWdCLFFBQTZCLENBQUM7SUFDOUQsSUFBQSxLQUFBLE9BQWtDLEtBQUssQ0FBQyxRQUFRLENBQVMsQ0FBQyxDQUFDLElBQUEsRUFBMUQsWUFBWSxRQUFBLEVBQUUsZUFBZSxRQUE2QixDQUFDO0lBQzVELElBQUEsS0FBQSxPQUEwQyxLQUFLLENBQUMsUUFBUSxDQUFTLENBQUMsQ0FBQyxJQUFBLEVBQWxFLGdCQUFnQixRQUFBLEVBQUUsbUJBQW1CLFFBQTZCLENBQUM7SUFDcEUsSUFBQSxLQUFBLE9BQTRDLEtBQUssQ0FBQyxRQUFRLENBQVMsQ0FBQyxDQUFDLElBQUEsRUFBcEUsaUJBQWlCLFFBQUEsRUFBRSxvQkFBb0IsUUFBNkIsQ0FBQztJQUU1RSw4REFBOEQ7SUFDeEQsSUFBQSxLQUFBLE9BQXFDLEtBQUssQ0FBQyxRQUFRLENBQWtCLEtBQUssQ0FBQyxjQUFjLENBQUMsSUFBQSxFQUF6RixjQUFjLFFBQUEsRUFBRSxpQkFBaUIsUUFBd0QsQ0FBQztJQUMzRixJQUFBLEtBQUEsT0FBc0MsS0FBSyxDQUFDLFFBQVEsQ0FBb0IsaUJBQWlCLENBQUMsS0FBSyxDQUFDLGNBQWMsQ0FBQyxDQUFDLElBQUEsRUFBL0csY0FBYyxRQUFBLEVBQUUsaUJBQWlCLFFBQThFLENBQUM7SUFDakgsSUFBQSxLQUFBLE9BQThCLEtBQUssQ0FBQyxRQUFRLENBQVMsQ0FBQyxDQUFDLElBQUEsRUFBdEQsVUFBVSxRQUFBLEVBQUUsYUFBYSxRQUE2QixDQUFDO0lBRXhELElBQUEsS0FBQSxPQUFrQyxLQUFLLENBQUMsUUFBUSxDQUFTLE1BQUEsS0FBSyxDQUFDLFlBQVksbUNBQUksbUJBQW1CLENBQUMsSUFBQSxFQUFsRyxZQUFZLFFBQUEsRUFBRSxlQUFlLFFBQXFFLENBQUM7SUFDcEcsSUFBQSxLQUFBLE9BQWdDLEtBQUssQ0FBQyxRQUFRLENBQVMsTUFBQSxLQUFLLENBQUMsV0FBVyxtQ0FBSSxrQkFBa0IsQ0FBQyxJQUFBLEVBQTlGLFdBQVcsUUFBQSxFQUFFLGNBQWMsUUFBbUUsQ0FBQztJQUNoRyxJQUFBLEtBQUEsT0FBNEIsS0FBSyxDQUFDLFFBQVEsQ0FBUyxLQUFLLENBQUMsTUFBTSxDQUFDLElBQUEsRUFBL0QsU0FBUyxRQUFBLEVBQUUsWUFBWSxRQUF3QyxDQUFDO0lBQ2pFLElBQUEsTUFBQSxPQUEwQixLQUFLLENBQUMsUUFBUSxDQUFTLEtBQUssQ0FBQyxLQUFLLENBQUMsSUFBQSxFQUE1RCxRQUFRLFNBQUEsRUFBRSxXQUFXLFNBQXVDLENBQUM7SUFDOUQsSUFBQSxNQUFBLE9BQThCLEtBQUssQ0FBQyxRQUFRLENBQVMsRUFBRSxDQUFDLElBQUEsRUFBdkQsVUFBVSxTQUFBLEVBQUUsYUFBYSxTQUE4QixDQUFDO0lBRS9ELElBQU0sY0FBYyxHQUFHLEtBQUssQ0FBQyxXQUFXLENBQUMsVUFBQyxTQUE2RjtRQUNySSxJQUFNLFNBQVMsNEJBQU8sT0FBTyxTQUFDLENBQUM7UUFDL0IsSUFBSSxLQUFLLEdBQUcsS0FBSyxDQUFDO1FBQ2xCLFNBQVMsQ0FBQyxPQUFPLENBQUMsVUFBQyxDQUFDLEVBQUMsQ0FBQyxFQUFDLENBQUM7WUFDdEIsK0RBQStEO1lBQy9ELEtBQUssR0FBRyxTQUFTLENBQUMsQ0FBQyxFQUFDLENBQUMsRUFBQyxDQUFDLENBQUMsSUFBSSxLQUFLLENBQUM7UUFDcEMsQ0FBQyxDQUFDLENBQUE7UUFDRixJQUFJLEtBQUssRUFBQyxDQUFDO1lBQ1QsVUFBVSxDQUFDLFNBQVMsQ0FBQyxDQUFDO1FBQ3hCLENBQUM7SUFDSCxDQUFDLEVBQUUsQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDO0lBRWQsMENBQTBDO0lBQzFDLEtBQUssQ0FBQyxTQUFTLENBQUM7UUFDZCxJQUFJLEtBQUssQ0FBQyxZQUFZLEtBQUssU0FBUztZQUFFLGVBQWUsQ0FBQyxLQUFLLENBQUMsWUFBWSxDQUFDLENBQUM7SUFDNUUsQ0FBQyxFQUFFLENBQUMsS0FBSyxDQUFDLFlBQVksQ0FBQyxDQUFDLENBQUM7SUFFekIsS0FBSyxDQUFDLFNBQVMsQ0FBQztRQUNkLElBQUksS0FBSyxDQUFDLFdBQVcsS0FBSyxTQUFTO1lBQUUsY0FBYyxDQUFDLEtBQUssQ0FBQyxXQUFXLENBQUMsQ0FBQztJQUN6RSxDQUFDLEVBQUUsQ0FBQyxLQUFLLENBQUMsV0FBVyxDQUFDLENBQUMsQ0FBQztJQUV4Qiw2QkFBNkI7SUFDN0IsS0FBSyxDQUFDLFNBQVMsQ0FBQztRQUNkLFlBQVksQ0FBQyxLQUFLLENBQUMsTUFBTSxHQUFHLENBQUMsS0FBSyxDQUFDLE1BQU0sS0FBSyxRQUFRLENBQUEsQ0FBQyxDQUFDLFlBQVksQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztJQUM3RSxDQUFDLEVBQUUsQ0FBQyxLQUFLLENBQUMsTUFBTSxFQUFFLEtBQUssQ0FBQyxNQUFNLEVBQUUsWUFBWSxDQUFDLENBQUMsQ0FBQztJQUUvQyxLQUFLLENBQUMsU0FBUyxDQUFDO1FBQ2QsV0FBVyxDQUFDLEtBQUssQ0FBQyxLQUFLLEdBQUcsQ0FBQyxLQUFLLENBQUMsTUFBTSxLQUFLLE9BQU8sQ0FBQSxDQUFDLENBQUMsV0FBVyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDO0lBQ3pFLENBQUMsRUFBRSxDQUFDLEtBQUssQ0FBQyxLQUFLLEVBQUUsS0FBSyxDQUFDLE1BQU0sRUFBRSxXQUFXLENBQUMsQ0FBQyxDQUFDO0lBRTdDLG1CQUFtQjtJQUNuQixLQUFLLENBQUMsU0FBUyxDQUFDO1FBQ2QsSUFBSSxLQUFLLENBQUMsSUFBSSxLQUFLLFNBQVMsSUFBSSxPQUFPLENBQUMsQ0FBQyxDQUFDLEdBQUcsS0FBSyxDQUFDLElBQUk7WUFDckQsVUFBVSxDQUFDLFVBQUMsQ0FBQyxZQUFLLE9BQUEsQ0FBQyxDQUFDLE1BQUEsS0FBSyxDQUFDLElBQUksbUNBQUcsQ0FBQyxFQUFFLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUEsRUFBQSxDQUFDLENBQUM7UUFDOUMsSUFBSSxLQUFLLENBQUMsSUFBSSxLQUFLLFNBQVMsSUFBSSxPQUFPLENBQUMsQ0FBQyxDQUFDLEdBQUcsS0FBSyxDQUFDLElBQUk7WUFDckQsVUFBVSxDQUFDLFVBQUMsQ0FBQyxZQUFLLE9BQUEsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsRUFBRSxNQUFBLEtBQUssQ0FBQyxJQUFJLG1DQUFHLENBQUMsQ0FBQyxDQUFDLENBQUEsRUFBQSxDQUFDLENBQUM7SUFDaEQsQ0FBQyxFQUFFLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQztJQUVkLG1CQUFtQjtJQUNuQixLQUFLLENBQUMsU0FBUyxDQUFDO1FBQ2QsSUFBTSxZQUFZLEdBQUcsVUFBQyxNQUF1QixFQUFFLElBQVksRUFBRSxVQUE4QjtZQUN6RixJQUFJLFVBQVUsR0FBRyxLQUFLLENBQUM7WUFDdkIsc0NBQXNDO1lBQ3RDLElBQU0sT0FBTyxHQUF1QixXQUFXLENBQVMsS0FBSyxDQUFDLElBQUksRUFBRSxJQUFJLENBQUMsQ0FBQztZQUMxRSxJQUFNLE9BQU8sR0FBdUIsV0FBVyxDQUFTLEtBQUssQ0FBQyxJQUFJLEVBQUUsSUFBSSxDQUFDLENBQUM7WUFDMUUsSUFBSSxPQUFPLEtBQUssU0FBUyxJQUFJLE1BQU0sQ0FBQyxDQUFDLENBQUMsR0FBRyxPQUFPLEVBQUMsQ0FBQztnQkFDaEQsVUFBVSxDQUFDLElBQUksQ0FBQyxHQUFHLENBQUMsT0FBTyxFQUFFLE1BQU0sQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDO2dCQUN4QyxVQUFVLEdBQUcsSUFBSSxDQUFDO1lBQ3BCLENBQUM7WUFDRCxJQUFJLE9BQU8sS0FBSyxTQUFTLElBQUksTUFBTSxDQUFDLENBQUMsQ0FBQyxHQUFHLE9BQU8sRUFBRSxDQUFDO2dCQUNqRCxVQUFVLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxNQUFNLENBQUMsQ0FBQyxDQUFDLEVBQUUsT0FBTyxDQUFDLENBQUM7Z0JBQ3hDLFVBQVUsR0FBRyxJQUFJLENBQUM7WUFDcEIsQ0FBQztZQUNELE9BQU8sVUFBVSxDQUFDO1FBQ3BCLENBQUMsQ0FBQTtRQUNELGNBQWMsQ0FBQyxZQUFZLENBQUMsQ0FBQztJQUMvQixDQUFDLEVBQUUsQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFBO0lBRWIsS0FBSyxDQUFDLFNBQVMsQ0FBQztRQUNkLElBQUksQ0FBQyxJQUFBLGdCQUFPLEVBQUMsY0FBYyxFQUFFLEtBQUssQ0FBQyxjQUFjLENBQUM7WUFDaEQsaUJBQWlCLENBQUMsS0FBSyxDQUFDLGNBQWMsQ0FBQyxDQUFDO0lBQzVDLENBQUMsRUFBRSxDQUFDLEtBQUssQ0FBQyxjQUFjLENBQUMsQ0FBQyxDQUFBO0lBRTFCLEtBQUssQ0FBQyxTQUFTLENBQUM7UUFDZCxJQUFJLENBQUMsSUFBQSxnQkFBTyxFQUFDLGNBQWMsRUFBRSxLQUFLLENBQUMsY0FBYyxDQUFDO1lBQ2hELGlCQUFpQixDQUFDLGlCQUFpQixDQUFDLEtBQUssQ0FBQyxjQUFjLENBQUMsQ0FBQyxDQUFDO0lBQy9ELENBQUMsRUFBRSxDQUFDLEtBQUssQ0FBQyxjQUFjLENBQUMsQ0FBQyxDQUFBO0lBRTFCLEtBQUssQ0FBQyxTQUFTLENBQUM7UUFDZCxVQUFVLENBQUMsY0FBYyxDQUFDLENBQUM7SUFDN0IsQ0FBQyxFQUFFLENBQUMsY0FBYyxDQUFDLENBQUMsQ0FBQTtJQUVwQixLQUFLLENBQUMsU0FBUyxDQUFDO1FBQ2QsVUFBVSxDQUFDLGNBQWMsQ0FBQyxDQUFDO0lBQzdCLENBQUMsRUFBRSxDQUFDLGNBQWMsQ0FBQyxDQUFDLENBQUE7SUFFcEIsK0JBQStCO0lBQy9CLEtBQUssQ0FBQyxTQUFTLENBQUM7UUFDZCxJQUFNLEdBQUcsR0FBRyxhQUFhLEdBQUcsRUFBRSxDQUFDO1FBQy9CLElBQU0sTUFBTSxHQUFHLFlBQVksQ0FBQztRQUM1QixJQUFJLFNBQVMsS0FBSyxHQUFHO1lBQ25CLFlBQVksQ0FBQyxHQUFHLENBQUMsQ0FBQztRQUNwQixJQUFJLFlBQVksS0FBSyxNQUFNO1lBQ3pCLGVBQWUsQ0FBQyxNQUFNLENBQUMsQ0FBQztJQUM1QixDQUFDLEVBQUMsQ0FBQyxZQUFZLEVBQUMsYUFBYSxDQUFDLENBQUMsQ0FBQTtJQUUvQixxQkFBcUI7SUFDckIsS0FBSyxDQUFDLFNBQVMsQ0FBQztRQUNkLElBQU0sSUFBSSxHQUFHLGdCQUFnQixHQUFHLENBQUMsS0FBSyxDQUFDLFlBQVksS0FBSyxNQUFNLENBQUMsQ0FBQyxDQUFDLENBQUMsVUFBVSxHQUFHLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxFQUFFLENBQUMsQ0FBQztRQUN4RixhQUFhLENBQUMsSUFBSSxDQUFDLENBQUM7SUFDdEIsQ0FBQyxFQUFFLENBQUMsZ0JBQWdCLEVBQUUsS0FBSyxDQUFDLFlBQVksRUFBRSxVQUFVLENBQUMsQ0FBQyxDQUFDO0lBRXZELHNCQUFzQjtJQUN0QixLQUFLLENBQUMsU0FBUyxDQUFDO1FBQ2QsSUFBTSxLQUFLLEdBQUcsaUJBQWlCLEdBQUcsQ0FBQyxDQUFDLEtBQUssQ0FBQyxZQUFZLEtBQUssT0FBTyxJQUFJLEtBQUssQ0FBQyxZQUFZLEtBQUssU0FBUyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsVUFBVSxHQUFHLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxFQUFFLENBQUMsQ0FBQztRQUNqSSxjQUFjLENBQUMsS0FBSyxDQUFDLENBQUM7SUFDeEIsQ0FBQyxFQUFFLENBQUMsaUJBQWlCLEVBQUUsS0FBSyxDQUFDLFlBQVksRUFBRSxVQUFVLENBQUMsQ0FBQyxDQUFDO0lBRXhELDJCQUEyQjtJQUMzQixLQUFLLENBQUMsU0FBUyxDQUFDO1FBQ2QsSUFBSSxLQUFLLENBQUMsT0FBTyxLQUFLLFdBQVcsSUFBSSxLQUFLLENBQUMsT0FBTyxLQUFLLGVBQWU7WUFBRSxPQUFPO1FBRS9FLElBQU0sZUFBZSxHQUFHLFVBQUMsTUFBZ0IsRUFBRSxNQUFtQixFQUFFLElBQXFELEVBQUUsSUFBWTtZQUNqSSwrRUFBK0U7WUFDL0UsSUFBTSxRQUFRLEdBQUcsc0JBQU8sQ0FBQyxHQUFHLENBQUMsTUFBTSxDQUFDLElBQUksQ0FBQyxDQUFDO1lBQzFDLElBQUksSUFBSSxLQUFLLFFBQVEsRUFBRSxDQUFDO2dCQUN0QixJQUFNLEtBQUssR0FBSSxJQUFJLENBQUMsQ0FBQyxNQUFNLENBQUMsZ0JBQWdCLEVBQUUsTUFBTSxDQUFDLGdCQUFnQixDQUFDLENBQUMsQ0FBQztnQkFDeEUsSUFBSSxLQUFLLEtBQUssU0FBUztvQkFBRSxNQUFNLENBQUMsSUFBSSxDQUFDLEtBQUssQ0FBQyxDQUFDO1lBQzlDLENBQUM7WUFDRCxPQUFPLE1BQU0sQ0FBQztRQUNoQixDQUFDLENBQUE7UUFFRCxJQUFNLGdCQUFnQixHQUF1QixjQUFjLENBQUMsR0FBRyxDQUFDLFVBQUMsT0FBTyxFQUFFLElBQUk7WUFDNUUsSUFBTSxJQUFJLEdBQUcsSUFBSSxDQUFDLEdBQUcsT0FBUixJQUFJLDJCQUFRLHlCQUFJLElBQUksQ0FBQyxPQUFPLENBQUMsTUFBTSxFQUFFLFVBQUUsTUFBTSxDQUFDLFVBQUMsTUFBZ0IsRUFBRSxNQUFtQixJQUFLLE9BQUEsZUFBZSxDQUFDLE1BQU0sRUFBRSxNQUFNLEVBQUUsTUFBTSxDQUFDLE1BQU0sRUFBRSxJQUFJLENBQUMsRUFBcEQsQ0FBb0QsRUFBRSxFQUFFLENBQUMsVUFBQyxDQUFDO1lBQ2pLLElBQU0sSUFBSSxHQUFHLElBQUksQ0FBQyxHQUFHLE9BQVIsSUFBSSwyQkFBUSx5QkFBSSxJQUFJLENBQUMsT0FBTyxDQUFDLE1BQU0sRUFBRSxVQUFFLE1BQU0sQ0FBQyxVQUFDLE1BQWdCLEVBQUUsTUFBbUIsSUFBSyxPQUFBLGVBQWUsQ0FBQyxNQUFNLEVBQUUsTUFBTSxFQUFFLE1BQU0sQ0FBQyxNQUFNLEVBQUUsSUFBSSxDQUFDLEVBQXBELENBQW9ELEVBQUUsRUFBRSxDQUFDLFVBQUMsQ0FBQztZQUNqSyxJQUFJLENBQUMsS0FBSyxDQUFDLElBQUksQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLElBQUksQ0FBQyxJQUFJLFFBQVEsQ0FBQyxJQUFJLENBQUMsSUFBSSxRQUFRLENBQUMsSUFBSSxDQUFDLEVBQUUsQ0FBQztnQkFDckUsSUFBSSxLQUFLLENBQUMsT0FBTyxLQUFLLFdBQVc7b0JBQUUsT0FBTyxDQUFDLElBQUksRUFBRSxJQUFJLENBQUMsQ0FBQztnQkFDdkQsK0VBQStFO3FCQUMxRSxJQUFJLElBQUksQ0FBQyxHQUFHLENBQUMsSUFBSSxDQUFDLElBQUksSUFBSSxDQUFDLEdBQUcsQ0FBQyxJQUFJLENBQUM7b0JBQUUsT0FBTyxDQUFDLENBQUMsRUFBRSxJQUFJLENBQUMsQ0FBQzs7b0JBQ3ZELE9BQU8sQ0FBQyxJQUFJLEVBQUUsQ0FBQyxDQUFDLENBQUM7WUFDeEIsQ0FBQztZQUNELE9BQU8sQ0FBQyxDQUFDLEVBQUMsQ0FBQyxDQUFDLENBQUM7UUFDZixDQUFDLENBQUMsQ0FBQztRQUVILElBQUksQ0FBQyxDQUFDLENBQUMsT0FBTyxDQUFDLGdCQUFnQixFQUFFLGNBQWMsQ0FBQztZQUFFLGlCQUFpQixDQUFDLGdCQUFnQixDQUFDLENBQUM7SUFDeEYsQ0FBQyxFQUFFLENBQUMsUUFBUSxFQUFFLEtBQUssQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDO0lBRTlCLEtBQUssQ0FBQyxTQUFTLENBQUM7UUFDZCxJQUFNLFVBQVUsR0FBYyxLQUFLLENBQVUsQ0FBQyxDQUFDLENBQUM7UUFDaEQsSUFBTSxPQUFPLEdBQUcsVUFBQyxJQUFvQixJQUFLLE9BQUEseUJBQUksSUFBSSxDQUFDLE9BQU8sQ0FBQyxNQUFNLEVBQUUsVUFBRSxJQUFJLENBQUMsVUFBQSxNQUFNLElBQUksT0FBQSxzQkFBTyxDQUFDLEdBQUcsQ0FBQyxJQUFJLENBQUMsS0FBSyxzQkFBTyxDQUFDLEdBQUcsQ0FBQyxNQUFNLENBQUMsSUFBSSxDQUFDLEVBQTlDLENBQThDLENBQUMsRUFBekYsQ0FBeUYsQ0FBQztRQUNwSSxVQUFVLENBQUMsQ0FBQyxDQUFDLEdBQUcsT0FBTyxDQUFDLE1BQU0sQ0FBQyxDQUFDO1FBQ2hDLFVBQVUsQ0FBQyxDQUFDLENBQUMsR0FBRyxPQUFPLENBQUMsT0FBTyxDQUFDLENBQUM7UUFDakMsV0FBVyxDQUFDLFVBQVUsQ0FBQyxDQUFDO0lBQzFCLENBQUMsRUFBRSxDQUFDLFFBQVEsQ0FBQyxDQUFDLENBQUM7SUFFZixnQkFBZ0I7SUFDaEIsS0FBSyxDQUFDLFNBQVMsQ0FBQztRQUNmLElBQUksRUFBRSxHQUFHLE9BQU8sQ0FBQyxDQUFDLENBQUMsR0FBRyxPQUFPLENBQUMsQ0FBQyxDQUFDLENBQUM7UUFDakMsSUFBSSxLQUFLLEdBQUcsT0FBTyxDQUFDLENBQUMsQ0FBQyxDQUFDO1FBRXRCLElBQUksRUFBRSxLQUFLLENBQUM7WUFDVixPQUFPO1FBRVQsSUFBSSxLQUFLLENBQUMsU0FBUyxLQUFLLEtBQUssRUFBRSxDQUFDO1lBQzlCLEVBQUUsR0FBRyxJQUFJLENBQUMsS0FBSyxDQUFDLE9BQU8sQ0FBQyxDQUFDLENBQUMsQ0FBQyxHQUFHLElBQUksQ0FBQyxLQUFLLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7WUFDckQsS0FBSyxHQUFHLElBQUksQ0FBQyxLQUFLLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7UUFDakMsQ0FBQztRQUVELElBQU0sS0FBSyxHQUFHLENBQUMsUUFBUSxHQUFHLFVBQVUsR0FBRyxXQUFXLENBQUMsR0FBRyxFQUFFLENBQUM7UUFFekQsU0FBUyxDQUFDLEtBQUssQ0FBQyxDQUFDO1FBQ2pCLFVBQVUsQ0FBQyxVQUFVLEdBQUcsS0FBSyxHQUFHLEtBQUssQ0FBRSxDQUFDO0lBQzFDLENBQUMsRUFBRSxDQUFDLE9BQU8sRUFBRSxVQUFVLEVBQUUsV0FBVyxFQUFFLEtBQUssQ0FBQyxTQUFTLEVBQUMsUUFBUSxDQUFDLENBQUMsQ0FBQztJQUVqRSxnQkFBZ0I7SUFDaEIsS0FBSyxDQUFDLFNBQVMsQ0FBQzs7UUFDZCxJQUFNLGNBQWMsR0FBRyxVQUFDLFVBQW9CLEVBQUUsV0FBcUIsRUFBRSxJQUFZO1lBQy9FLElBQU0sRUFBRSxHQUFHLE9BQU8sQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsR0FBRyxPQUFPLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7WUFDL0MsSUFBTSxLQUFLLEdBQUcsQ0FBQyxTQUFTLEdBQUcsU0FBUyxHQUFHLFlBQVksQ0FBQyxHQUFHLENBQUMsRUFBRSxLQUFLLENBQUMsQ0FBQSxDQUFDLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQyxFQUFFLENBQUMsQ0FBQztZQUNoRixVQUFVLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxLQUFLLENBQUM7WUFDMUIsV0FBVyxDQUFDLElBQUksQ0FBQyxHQUFHLFNBQVMsR0FBRyxZQUFZLEdBQUcsT0FBTyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsQ0FBQyxHQUFHLEtBQUssQ0FBQztRQUMxRSxDQUFDLENBQUE7UUFDRCxvQkFBb0I7UUFDcEIsSUFBTSxRQUFRLDRCQUFPLE1BQU0sU0FBQyxDQUFDO1FBQzdCLElBQU0sU0FBUyw0QkFBTyxPQUFPLFNBQUMsQ0FBQzs7WUFDL0IsS0FBa0IsSUFBQSxLQUFBLFNBQUEsc0JBQU8sQ0FBQyxNQUFNLEVBQUUsQ0FBQSxnQkFBQTtnQkFBOUIsSUFBTSxJQUFJLFdBQUE7Z0JBQ1osY0FBYyxDQUFDLFFBQVEsRUFBRSxTQUFTLEVBQUUsSUFBSSxDQUFDLENBQUM7YUFBQTs7Ozs7Ozs7O1FBQzVDLFNBQVMsQ0FBQyxRQUFRLENBQUMsQ0FBQztRQUNwQixVQUFVLENBQUMsU0FBUyxDQUFDLENBQUM7SUFFeEIsQ0FBQyxFQUFFLENBQUMsT0FBTyxFQUFFLFNBQVMsRUFBRSxZQUFZLEVBQUUsU0FBUyxDQUFDLENBQUMsQ0FBQztJQUVsRCxLQUFLLENBQUMsU0FBUyxDQUFDLGNBQVEsYUFBYSxDQUFDLFVBQUMsQ0FBQyxJQUFLLE9BQUEsQ0FBQyxHQUFDLENBQUMsRUFBSCxDQUFHLENBQUMsQ0FBQSxDQUFDLENBQUMsRUFBRSxDQUFDLE1BQU0sRUFBQyxPQUFPLEVBQUMsTUFBTSxFQUFDLE9BQU8sQ0FBQyxDQUFDLENBQUE7SUFFckYsc0JBQXNCO0lBQ3RCLEtBQUssQ0FBQyxTQUFTLENBQUM7UUFDZCxJQUFJLFNBQVMsQ0FBQztRQUNkLElBQUksS0FBSyxDQUFDLGNBQWMsSUFBSSxJQUFJLEVBQUUsQ0FBQztZQUNqQyxRQUFRLFlBQVksRUFBQyxDQUFDO2dCQUNwQixLQUFLLEtBQUs7b0JBQ1IsU0FBUyxHQUFHLE1BQU0sQ0FBQztvQkFDbkIsTUFBTTtnQkFDUixLQUFLLFFBQVE7b0JBQ1gsU0FBUyxHQUFHLFNBQVMsQ0FBQztvQkFDdEIsTUFBTTtnQkFDUjtvQkFDRSxTQUFTLEdBQUcsV0FBVyxDQUFDO1lBQzVCLENBQUM7UUFDSCxDQUFDOztZQUFNLFNBQVMsR0FBRyxLQUFLLENBQUMsY0FBYyxDQUFDO1FBQ3hDLGFBQWEsQ0FBQyxTQUFTLENBQUMsQ0FBQztJQUMzQixDQUFDLEVBQUUsQ0FBQyxZQUFZLEVBQUUsS0FBSyxDQUFDLGNBQWMsQ0FBQyxDQUFDLENBQUE7SUFFeEMsK0JBQStCO0lBQy9CLEtBQUssQ0FBQyxTQUFTLENBQUM7UUFDZCxJQUFNLFdBQVcsR0FBRyxVQUFDLENBQWEsSUFBTyxJQUFJLFlBQVksQ0FBQyxPQUFPLENBQUMsVUFBVTtZQUFFLENBQUMsQ0FBQyxjQUFjLEVBQUUsQ0FBQSxDQUFDLENBQUMsQ0FBQTtRQUNsRyxRQUFRLENBQUMsSUFBSSxDQUFDLGdCQUFnQixDQUFDLE9BQU8sRUFBRSxXQUFXLEVBQUUsRUFBQyxPQUFPLEVBQUMsS0FBSyxFQUFDLENBQUMsQ0FBQztRQUN0RSxPQUFPLGNBQU0sT0FBQSxRQUFRLENBQUMsSUFBSSxDQUFDLG1CQUFtQixDQUFDLE9BQU8sRUFBRSxXQUFXLENBQUMsRUFBdkQsQ0FBdUQsQ0FBQztJQUN6RSxDQUFDLEVBQUUsRUFBRSxDQUFDLENBQUM7SUFFUCw0Q0FBNEM7SUFDNUMsS0FBSyxDQUFDLFNBQVMsQ0FBQztRQUNkLHdLQUF3SztRQUN4SyxJQUFJLENBQUMsVUFBVTtZQUFFLE9BQU87UUFDeEIsK0ZBQStGO1FBQy9GLFlBQVksQ0FBQyxZQUFZLENBQUMsT0FBTyxDQUFDLE9BQU8sQ0FBQyxDQUFDO1FBQzNDLHNFQUFzRTtRQUN0RSxZQUFZLENBQUMsT0FBTyxDQUFDLE9BQU8sR0FBRyxVQUFVLENBQUM7O1lBQ3hDLElBQU0sRUFBRSxHQUFHLE1BQUEsWUFBWSxDQUFDLE9BQU8sQ0FBQyxTQUFTLG1DQUFJLElBQUksQ0FBQztZQUNsRCxJQUFNLE9BQU8sR0FBRyxRQUFRLENBQUMsY0FBYyxDQUFDLEVBQUUsQ0FBQyxDQUFDO1lBQzVDLElBQUksT0FBTyxJQUFJLElBQUksRUFBRSxDQUFDO2dCQUNwQixPQUFPLENBQUMsS0FBSyxDQUFDLGtEQUEyQyxFQUFFLENBQUUsQ0FBQyxDQUFDO1lBQ2pFLENBQUM7aUJBQU0sQ0FBQztnQkFDTixXQUFXLENBQUMsT0FBTyxDQUFDLENBQUMsSUFBSSxDQUFDLFVBQUMsTUFBeUI7b0JBQ2xELFFBQVEsQ0FBQyxJQUFJLENBQUMsV0FBVyxDQUFDLE1BQU0sQ0FBQyxDQUFDO29CQUNsQyxJQUFNLFNBQVMsR0FBRyxNQUFNLENBQUMsU0FBUyxDQUFDLFdBQVcsQ0FBQyxDQUFDLE9BQU8sQ0FBQyxrQkFBa0IsRUFBRSwrQkFBK0IsQ0FBQyxDQUFDO29CQUM3RyxJQUFNLGFBQWEsR0FBRyxRQUFRLENBQUMsYUFBYSxDQUFDLEdBQUcsQ0FBQyxDQUFDO29CQUNsRCxhQUFhLENBQUMsSUFBSSxHQUFHLFNBQVMsQ0FBQztvQkFDL0IsYUFBYSxDQUFDLFFBQVEsR0FBRyxVQUFHLEVBQUUsU0FBTSxDQUFDO29CQUNyQyxRQUFRLENBQUMsSUFBSSxDQUFDLFdBQVcsQ0FBQyxhQUFhLENBQUMsQ0FBQztvQkFDekMsYUFBYSxDQUFDLEtBQUssRUFBRSxDQUFDO29CQUN0QixvQ0FBb0M7b0JBQ3BDLE1BQU0sQ0FBQyxHQUFHLENBQUMsZUFBZSxDQUFDLFNBQVMsQ0FBQyxDQUFDO29CQUN0QyxRQUFRLENBQUMsSUFBSSxDQUFDLFdBQVcsQ0FBQyxhQUFhLENBQUMsQ0FBQztvQkFDekMsUUFBUSxDQUFDLElBQUksQ0FBQyxXQUFXLENBQUMsTUFBTSxDQUFDLENBQUM7Z0JBQ3BDLENBQUMsQ0FBQyxDQUFDO1lBQ0wsQ0FBQztZQUNELGFBQWEsQ0FBQyxLQUFLLENBQUMsQ0FBQztZQUNyQixJQUFJLEtBQUssQ0FBQyxpQkFBaUIsS0FBSyxTQUFTO2dCQUFFLEtBQUssQ0FBQyxpQkFBaUIsRUFBRSxDQUFDO1FBQ3ZFLENBQUMsRUFBRSxFQUFFLENBQUMsQ0FBQztJQUNULENBQUMsQ0FBQyxDQUFDO0lBRUgsdUVBQXVFO0lBQ3ZFLElBQU0seUJBQXlCLEdBQUcsS0FBSyxDQUFDLFdBQVcsQ0FBQyxVQUFDLFNBQWlCOztRQUNwRSxJQUFNLFdBQVcsR0FBRyxLQUFLLENBQUMsTUFBTSxLQUFLLFFBQVEsQ0FBQyxDQUFDLENBQUMsU0FBUyxDQUFDLENBQUMsQ0FBQyxDQUFDLE1BQUEsS0FBSyxDQUFDLFlBQVksbUNBQUksbUJBQW1CLENBQUMsQ0FBQztRQUN4RyxJQUFJLENBQUMsVUFBVTtZQUFFLFlBQVksQ0FBQyxPQUFPLENBQUMsV0FBVyxHQUFHLElBQUksQ0FBQyxHQUFHLENBQUMsU0FBUyxHQUFHLFdBQVcsRUFBRSxDQUFDLENBQUMsQ0FBQztRQUN6RixJQUFJLEtBQUssQ0FBQyxNQUFNLEtBQUssUUFBUTtZQUFFLE9BQU87UUFDdEMsSUFBTSxhQUFhLEdBQUcsSUFBSSxDQUFDLEdBQUcsQ0FBQyxTQUFTLEVBQUUsV0FBVyxDQUFDLENBQUM7UUFDdkQsSUFBSSxZQUFZLEtBQUssYUFBYTtZQUFFLGVBQWUsQ0FBQyxhQUFhLENBQUMsQ0FBQztJQUNyRSxDQUFDLEVBQUMsQ0FBQyxLQUFLLENBQUMsWUFBWSxFQUFFLGVBQWUsRUFBRSxZQUFZLEVBQUUsS0FBSyxDQUFDLE1BQU0sRUFBRSxVQUFVLENBQUMsQ0FBQyxDQUFDO0lBRWpGLElBQU0sd0JBQXdCLEdBQUcsS0FBSyxDQUFDLFdBQVcsQ0FBQyxVQUFDLFFBQWdCLEVBQUUsV0FBbUI7O1FBQ3ZGLElBQUksUUFBUSxHQUFHLENBQUMsRUFBRSxDQUFDO1lBQ2pCLFlBQVksQ0FBQyxPQUFPLENBQUMsWUFBWSxDQUFDLE1BQU0sQ0FBQyxXQUFXLENBQUMsQ0FBQztZQUN0RCxPQUFPO1FBQ1QsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLE1BQU0sS0FBSyxPQUFPO1lBQUUsT0FBTztRQUNyQyxJQUFNLFlBQVksR0FBRyxJQUFJLENBQUMsR0FBRyxDQUFDLFFBQVEsRUFBRSxNQUFBLEtBQUssQ0FBQyxXQUFXLG1DQUFJLGtCQUFrQixDQUFDLENBQUM7UUFDakYsNkhBQTZIO1FBQzdILFlBQVksQ0FBQyxZQUFZLENBQUMsT0FBTyxDQUFDLE9BQU8sQ0FBQyxDQUFDO1FBQzNDLFlBQVksQ0FBQyxPQUFPLENBQUMsWUFBWSxDQUFDLEdBQUcsQ0FBQyxXQUFXLEVBQUUsWUFBWSxDQUFDLENBQUM7UUFFakUsdUVBQXVFO1FBQ3ZFLFlBQVksQ0FBQyxPQUFPLENBQUMsT0FBTyxHQUFHLFVBQVUsQ0FBQztZQUN4QyxJQUFNLGdCQUFnQixHQUFHLElBQUksQ0FBQyxHQUFHLE9BQVIsSUFBSSwyQkFBUSxZQUFZLENBQUMsT0FBTyxDQUFDLFlBQVksQ0FBQyxNQUFNLEVBQUUsVUFBQyxDQUFDO1lBQ2pGLElBQUksV0FBVyxLQUFLLGdCQUFnQjtnQkFBRSxjQUFjLENBQUMsZ0JBQWdCLENBQUMsQ0FBQztRQUN6RSxDQUFDLEVBQUUsRUFBRSxDQUFDLENBQUM7SUFDVCxDQUFDLEVBQUMsQ0FBQyxLQUFLLENBQUMsV0FBVyxFQUFFLGNBQWMsRUFBRSxXQUFXLEVBQUUsS0FBSyxDQUFDLE1BQU0sQ0FBQyxDQUFDLENBQUM7SUFFaEUsNEVBQTRFO0lBQzVFLElBQU0sYUFBYSxHQUFHLEtBQUssQ0FBQyxXQUFXLENBQUMsVUFBQyxDQUFTO1FBQ2hELElBQUksRUFBRSxHQUFHLENBQUMsQ0FBQyxHQUFHLE9BQU8sQ0FBQyxHQUFHLE1BQU0sQ0FBQztRQUNoQyxJQUFJLEtBQUssQ0FBQyxTQUFTLEtBQUssS0FBSztZQUMzQixFQUFFLEdBQUcsSUFBSSxDQUFDLEdBQUcsQ0FBQyxFQUFFLEVBQUUsQ0FBQyxDQUFDLEdBQUcsT0FBTyxDQUFDLEdBQUcsTUFBTSxDQUFDLENBQUM7UUFDNUMsT0FBTyxFQUFFLENBQUM7SUFDWixDQUFDLEVBQUMsQ0FBQyxPQUFPLEVBQUMsTUFBTSxFQUFDLEtBQUssQ0FBQyxTQUFTLENBQUMsQ0FBQyxDQUFDO0lBRXBDLDJFQUEyRTtJQUMzRSxJQUFNLGFBQWEsR0FBRyxLQUFLLENBQUMsV0FBVyxDQUFDLFVBQUMsQ0FBUyxFQUFFLENBQTBCO1FBQzVFLElBQU0sSUFBSSxHQUFHLENBQUMsT0FBTSxDQUFDLENBQUMsQ0FBQyxLQUFLLFFBQVEsQ0FBQyxDQUFDLENBQUMsQ0FBQyxzQkFBTyxDQUFDLEdBQUcsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDO1FBQzNELE9BQU8sQ0FBQyxDQUFDLEdBQUcsT0FBTyxDQUFDLElBQUksQ0FBQyxDQUFDLEdBQUcsTUFBTSxDQUFDLElBQUksQ0FBQyxDQUFDO0lBQzVDLENBQUMsRUFBQyxDQUFDLE9BQU8sRUFBQyxNQUFNLENBQUMsQ0FBQyxDQUFDO0lBRXBCLElBQU0sS0FBSyxHQUFHLEtBQUssQ0FBQyxXQUFXLENBQUM7UUFDOUIsVUFBVSxDQUFDLGNBQWMsQ0FBQyxDQUFDO1FBQzNCLFVBQVUsQ0FBQyxjQUFjLENBQUMsQ0FBQztJQUM3QixDQUFDLEVBQUUsQ0FBQyxjQUFjLEVBQUUsY0FBYyxDQUFDLENBQUMsQ0FBQztJQUVyQyxnREFBZ0Q7SUFDaEQsSUFBTSxVQUFVLEdBQUcsS0FBSyxDQUFDLFdBQVcsQ0FBQyxVQUFDLEtBQWE7UUFDakQsSUFBSSxFQUFFLEdBQUcsS0FBSyxHQUFHLE1BQU0sR0FBRyxPQUFPLENBQUM7UUFDbEMsSUFBSSxLQUFLLENBQUMsU0FBUyxLQUFLLEtBQUssRUFBRSxDQUFDO1lBQzlCLElBQU0sQ0FBQyxHQUFHLENBQUMsS0FBSyxLQUFLLENBQUMsQ0FBQSxDQUFDLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQyxHQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDakQsRUFBRSxHQUFHLElBQUksQ0FBQyxLQUFLLENBQUMsQ0FBQyxDQUFDLEdBQUcsTUFBTSxHQUFHLE9BQU8sQ0FBQztRQUN4QyxDQUFDO1FBQ0QsT0FBTyxFQUFFLENBQUM7SUFDWixDQUFDLEVBQUUsQ0FBQyxNQUFNLEVBQUMsT0FBTyxFQUFDLEtBQUssQ0FBQyxTQUFTLEVBQUUsT0FBTyxDQUFDLENBQUMsQ0FBQTtJQUU3QyxnREFBZ0Q7SUFDaEQsSUFBTSxVQUFVLEdBQUcsS0FBSyxDQUFDLFdBQVcsQ0FBQyxVQUFDLEtBQWEsRUFBRSxDQUF3QjtRQUMzRSxJQUFNLElBQUksR0FBRyxDQUFDLE9BQU0sQ0FBQyxDQUFDLENBQUMsS0FBSyxRQUFRLENBQUMsQ0FBQyxDQUFDLENBQUMsc0JBQU8sQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztRQUMzRCxPQUFPLEtBQUssR0FBRyxNQUFNLENBQUMsSUFBSSxDQUFDLEdBQUcsT0FBTyxDQUFDLElBQUksQ0FBQyxDQUFDO0lBQzlDLENBQUMsRUFBRSxDQUFDLE1BQU0sRUFBQyxPQUFPLENBQUMsQ0FBQyxDQUFDO0lBRXJCLG1GQUFtRjtJQUNuRixJQUFNLFlBQVksR0FBRyxLQUFLLENBQUMsV0FBVyxDQUFDLFVBQUMsS0FBYTtRQUNuRCxJQUFJLEtBQUssSUFBSSxDQUFDO1lBQ1osT0FBTyxJQUFJLENBQUMsR0FBRyxDQUFDLEtBQUssR0FBRyxVQUFVLEVBQUUsUUFBUSxHQUFHLFdBQVcsQ0FBQyxDQUFDOztZQUU1RCxPQUFPLElBQUksQ0FBQyxHQUFHLENBQUMsVUFBVSxFQUFFLFFBQVEsR0FBRyxXQUFXLEdBQUcsS0FBSyxDQUFDLENBQUM7SUFDaEUsQ0FBQyxFQUFFLENBQUMsVUFBVSxFQUFDLFdBQVcsRUFBQyxRQUFRLENBQUMsQ0FBQyxDQUFDO0lBRXRDLG1GQUFtRjtJQUNuRixJQUFNLFlBQVksR0FBRyxLQUFLLENBQUMsV0FBVyxDQUFDLFVBQUMsS0FBYTtRQUNuRCxJQUFJLEtBQUssSUFBSSxDQUFDO1lBQ1osT0FBTyxJQUFJLENBQUMsR0FBRyxDQUFDLEtBQUssR0FBRyxTQUFTLEVBQUUsU0FBUyxHQUFHLFlBQVksQ0FBQyxDQUFDOztZQUU3RCxPQUFPLElBQUksQ0FBQyxHQUFHLENBQUMsU0FBUyxFQUFFLFNBQVMsR0FBRyxZQUFZLEdBQUcsS0FBSyxDQUFDLENBQUM7SUFDakUsQ0FBQyxFQUFFLENBQUMsU0FBUyxFQUFDLFlBQVksRUFBQyxTQUFTLENBQUMsQ0FBQyxDQUFDO0lBRXZDLElBQU0sT0FBTyxHQUFHLEtBQUssQ0FBQyxXQUFXLENBQUMsVUFBQyxHQUFXLEVBQUUsQ0FBZTtRQUMzRCxXQUFXLENBQUMsSUFBQSw2QkFBVSxHQUFFLENBQUMsQ0FBQztRQUMxQixJQUFJLENBQUMsSUFBSSxJQUFJO1lBQ1QsSUFBSSxDQUFDLE9BQU8sQ0FBQyxHQUFHLENBQUMsR0FBRyxFQUFFLENBQUMsQ0FBQyxDQUFBOztZQUV4QixJQUFJLENBQUMsT0FBTyxDQUFDLE1BQU0sQ0FBQyxHQUFHLENBQUMsQ0FBQTtJQUNoQyxDQUFDLEVBQUUsRUFBRSxDQUFDLENBQUE7SUFFTixJQUFNLE9BQU8sR0FBRyxLQUFLLENBQUMsV0FBVyxDQUFDLFVBQUMsQ0FBYztRQUMvQyxJQUFNLEdBQUcsR0FBRyxJQUFBLDZCQUFVLEdBQUUsQ0FBQztRQUN6QixPQUFPLENBQUMsR0FBRyxFQUFFLENBQUMsQ0FBQyxDQUFDO1FBQ2hCLE9BQU8sR0FBRyxDQUFDO0lBQ2IsQ0FBQyxFQUFFLEVBQUUsQ0FBQyxDQUFDO0lBRVAsSUFBTSxTQUFTLEdBQUcsS0FBSyxDQUFDLFdBQVcsQ0FBQyxVQUFDLEdBQVcsRUFBRSxNQUEyQjtRQUN6RSxJQUFNLE1BQU0sR0FBRyxJQUFJLENBQUMsT0FBTyxDQUFDLEdBQUcsQ0FBQyxHQUFHLENBQUMsQ0FBQztRQUNyQyxJQUFJLE1BQU0sS0FBSyxTQUFTO1lBQ3BCLE9BQU87UUFFWCxNQUFNLENBQUMsTUFBTSxHQUFHLE1BQU0sQ0FBQztRQUN2QixJQUFJLENBQUMsT0FBTyxDQUFDLEdBQUcsQ0FBQyxHQUFHLEVBQUUsTUFBTSxDQUFDLENBQUM7SUFDbEMsQ0FBQyxFQUFFLEVBQUUsQ0FBQyxDQUFDO0lBRVAsU0FBUyx3QkFBd0IsQ0FBQyxPQUE2QjtRQUM3RCxJQUFNLElBQUksR0FBRyxhQUFhLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQyxDQUFDO1FBQ3RDLElBQU0sZ0JBQWdCLEdBQUcsVUFBQyxNQUE4RCxFQUFFLE1BQW1CO1lBQzNHLElBQU0sVUFBVSxHQUFHLE1BQU0sQ0FBQyxTQUFTLENBQUMsSUFBSSxFQUFFLENBQUMsQ0FBQyxDQUFDO1lBQzdDLElBQUksVUFBVSxLQUFLLFNBQVM7Z0JBQUUsT0FBTyxNQUFNLENBQUM7WUFDNUMsSUFBTSxhQUFhLEdBQUcsVUFBVSxDQUFDLE1BQU0sQ0FBQyxVQUFDLE1BQTZELEVBQUUsRUFBRTtnQkFDeEcsSUFBTSxLQUFLLEdBQUcsQ0FBQyxVQUFVLENBQUMsRUFBRSxDQUFDLENBQUMsQ0FBQyxDQUFDLEVBQUUsVUFBVSxDQUFDLEVBQUUsQ0FBQyxDQUFDLENBQUMsRUFBRSxzQkFBTyxDQUFDLEdBQUcsQ0FBQyxNQUFNLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDO2dCQUMvRSxJQUFNLFNBQVMsR0FBRyxTQUFBLENBQUMsS0FBSyxDQUFDLENBQUMsQ0FBQyxHQUFHLE9BQU8sQ0FBQyxDQUFDLENBQUMsRUFBRSxDQUFDLENBQUEsR0FBRyxTQUFBLENBQUMsS0FBSyxDQUFDLENBQUMsQ0FBQyxHQUFHLE9BQU8sQ0FBQyxDQUFDLENBQUMsRUFBRSxDQUFDLENBQUEsQ0FBQztnQkFDeEUsSUFBSSxNQUFNLENBQUMsTUFBTSxLQUFLLFNBQVMsSUFBSSxTQUFTLEdBQUcsTUFBTSxDQUFDLE1BQU07b0JBQUUsT0FBTyxFQUFDLEVBQUUsRUFBRSxFQUFFLENBQUMsRUFBRSxLQUFLLENBQUMsQ0FBQyxDQUFDLEVBQUUsQ0FBQyxFQUFFLEtBQUssQ0FBQyxDQUFDLENBQUMsRUFBQyxFQUFFLE1BQU0sRUFBRSxTQUFTLEVBQUMsQ0FBQztnQkFDMUgsT0FBTyxNQUFNLENBQUM7WUFFaEIsQ0FBQyxFQUFFLEVBQUUsRUFBRSxFQUFFLEVBQUMsQ0FBQyxFQUFDLENBQUMsRUFBRSxDQUFDLEVBQUMsQ0FBQyxFQUFDLEVBQUUsTUFBTSxFQUFFLFNBQVMsRUFBRSxDQUFDLENBQUM7WUFDMUMsSUFBSSxhQUFhLENBQUMsTUFBTSxLQUFLLFNBQVMsSUFBSSxDQUFDLE1BQU0sQ0FBQyxNQUFNLEtBQUssU0FBUyxJQUFJLGFBQWEsQ0FBQyxNQUFNLEdBQUcsTUFBTSxDQUFDLE1BQU0sQ0FBQztnQkFBRSxPQUFPLGFBQWEsQ0FBQztZQUN0SSxPQUFPLE1BQU0sQ0FBQztRQUNoQixDQUFDLENBQUE7UUFFRCxPQUFPLHlCQUFJLElBQUksQ0FBQyxPQUFPLENBQUMsTUFBTSxFQUFFLFVBQUUsTUFBTSxDQUFDLFVBQUMsTUFBNkQsRUFBRSxNQUFNLElBQUssT0FBQSxnQkFBZ0IsQ0FBQyxNQUFNLEVBQUUsTUFBTSxDQUFDLEVBQWhDLENBQWdDLEVBQUUsRUFBRSxFQUFFLEVBQUUsRUFBQyxDQUFDLEVBQUMsQ0FBQyxFQUFFLENBQUMsRUFBQyxDQUFDLEVBQUMsRUFBRSxNQUFNLEVBQUUsU0FBUyxFQUFFLENBQUMsQ0FBQyxFQUFFLENBQUM7SUFDbE0sQ0FBQztJQUVELElBQU0sY0FBYyxHQUFHLEtBQUssQ0FBQyxXQUFXLENBQUMsVUFBQyxPQUFrQjtRQUMxRCxJQUFNLEdBQUcsR0FBRyxJQUFBLDZCQUFVLEdBQUUsQ0FBQztRQUN6QixRQUFRLENBQUMsT0FBTyxDQUFDLEdBQUcsQ0FBQyxHQUFHLEVBQUMsT0FBTyxDQUFDLENBQUE7UUFDakMsT0FBTyxHQUFHLENBQUM7SUFDYixDQUFDLEVBQUMsRUFBRSxDQUFDLENBQUM7SUFFTixJQUFNLFlBQVksR0FBRyxLQUFLLENBQUMsV0FBVyxDQUFDLFVBQUMsR0FBVztRQUNqRCxRQUFRLENBQUMsT0FBTyxDQUFDLE1BQU0sQ0FBQyxHQUFHLENBQUMsQ0FBQTtJQUM5QixDQUFDLEVBQUMsRUFBRSxDQUFDLENBQUM7SUFFTixJQUFNLFlBQVksR0FBRyxLQUFLLENBQUMsV0FBVyxDQUFDLFVBQUMsR0FBVyxFQUFDLE9BQWtCO1FBQ3BFLFFBQVEsQ0FBQyxPQUFPLENBQUMsR0FBRyxDQUFDLEdBQUcsRUFBQyxPQUFPLENBQUMsQ0FBQTtJQUNuQyxDQUFDLEVBQUMsRUFBRSxDQUFDLENBQUM7SUFFTixJQUFNLFlBQVksR0FBRyxLQUFLLENBQUMsV0FBVyxDQUFDLFVBQUMsQ0FBUztRQUMvQyxJQUFJLENBQUMsS0FBSyxPQUFPO1lBQUUsS0FBSyxFQUFFLENBQUM7YUFDdEIsSUFBSSxDQUFDLEtBQUssVUFBVSxFQUFFLENBQUM7WUFBQyxJQUFJLEtBQUssQ0FBQyxhQUFhLEtBQUssU0FBUztnQkFBRSxLQUFLLENBQUMsYUFBYSxDQUFDLE9BQU8sQ0FBQyxDQUFDO1FBQUMsQ0FBQzthQUM5RixJQUFJLENBQUMsS0FBSyxTQUFTLEVBQUUsQ0FBQztZQUN6QixhQUFhLENBQUMsSUFBSSxDQUFDLENBQUM7WUFDcEIsSUFBSSxLQUFLLENBQUMsU0FBUyxLQUFLLFNBQVM7Z0JBQUUsWUFBWSxDQUFDLE9BQU8sQ0FBQyxTQUFTLEdBQUcsS0FBSyxDQUFDLFNBQVMsQ0FBQyxZQUFZLENBQUMsT0FBTyxDQUFDLFdBQVcsQ0FBQyxDQUFDO1FBQ3hILENBQUM7O1lBQ0ksZUFBZSxDQUFDLENBQWtGLENBQUMsQ0FBQTtJQUMxRyxDQUFDLEVBQUUsQ0FBQyxPQUFPLEVBQUUsS0FBSyxFQUFFLEtBQUssQ0FBQyxhQUFhLENBQUMsQ0FBQyxDQUFDO0lBRTFDLElBQU0scUJBQXFCLEdBQUcsS0FBSyxDQUFDLFdBQVcsQ0FBQyxVQUFDLFVBQTRCO1FBQzNFLElBQU0sZUFBZSxHQUFHLFVBQUMsTUFBZ0IsRUFBRSxNQUFtQixFQUFFLElBQXFELEVBQUUsSUFBWTtZQUNqSSwrRUFBK0U7WUFDL0UsSUFBTSxRQUFRLEdBQUcsc0JBQU8sQ0FBQyxHQUFHLENBQUMsTUFBTSxDQUFDLElBQUksQ0FBQyxDQUFDO1lBQzFDLElBQUksSUFBSSxLQUFLLFFBQVEsRUFBRSxDQUFDO2dCQUN0QixJQUFNLEtBQUssR0FBSSxJQUFJLENBQUMsVUFBVSxDQUFDLENBQUM7Z0JBQ2hDLElBQUksS0FBSyxLQUFLLFNBQVM7b0JBQUUsTUFBTSxDQUFDLElBQUksQ0FBQyxLQUFLLENBQUMsQ0FBQztZQUM5QyxDQUFDO1lBQ0QsT0FBTyxNQUFNLENBQUM7UUFDaEIsQ0FBQyxDQUFBO1FBQ0QsT0FBTyxPQUFPLENBQUMsR0FBRyxDQUFDLFVBQUMsU0FBUyxFQUFFLElBQUk7WUFDakMsSUFBTSxJQUFJLEdBQUcsSUFBSSxDQUFDLEdBQUcsT0FBUixJQUFJLDJCQUFRLHlCQUFJLElBQUksQ0FBQyxPQUFPLENBQUMsTUFBTSxFQUFFLFVBQUUsTUFBTSxDQUFDLFVBQUMsTUFBZ0IsRUFBRSxNQUFtQixJQUFLLE9BQUEsZUFBZSxDQUFDLE1BQU0sRUFBRSxNQUFNLEVBQUUsTUFBTSxDQUFDLE1BQU0sRUFBRSxJQUFJLENBQUMsRUFBcEQsQ0FBb0QsRUFBRSxFQUFFLENBQUMsVUFBQyxDQUFDO1lBQ2pLLElBQU0sSUFBSSxHQUFHLElBQUksQ0FBQyxHQUFHLE9BQVIsSUFBSSwyQkFBUSx5QkFBSSxJQUFJLENBQUMsT0FBTyxDQUFDLE1BQU0sRUFBRSxVQUFFLE1BQU0sQ0FBQyxVQUFDLE1BQWdCLEVBQUUsTUFBbUIsSUFBSyxPQUFBLGVBQWUsQ0FBQyxNQUFNLEVBQUUsTUFBTSxFQUFFLE1BQU0sQ0FBQyxNQUFNLEVBQUUsSUFBSSxDQUFDLEVBQXBELENBQW9ELEVBQUUsRUFBRSxDQUFDLFVBQUMsQ0FBQztZQUNqSyxJQUFJLENBQUMsS0FBSyxDQUFDLElBQUksQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLElBQUksQ0FBQyxJQUFJLFFBQVEsQ0FBQyxJQUFJLENBQUMsSUFBSSxRQUFRLENBQUMsSUFBSSxDQUFDO2dCQUFFLE9BQU8sQ0FBQyxJQUFJLEVBQUUsSUFBSSxDQUFDLENBQUM7WUFDMUYsT0FBTyxPQUFPLENBQUMsSUFBSSxDQUFDLENBQUM7UUFDdkIsQ0FBQyxDQUFDLENBQUM7SUFDTCxDQUFDLEVBQUUsQ0FBQyxRQUFRLEVBQUUsT0FBTyxDQUFDLENBQUMsQ0FBQztJQUV4QixTQUFTLGdCQUFnQixDQUFDLEdBQVE7O1FBQzVCLElBQUksS0FBSyxDQUFDLElBQUksS0FBSyxTQUFTLElBQUksQ0FBQyxLQUFLLENBQUMsSUFBSTtZQUN2QyxPQUFPO1FBQ1gsSUFBSSxDQUFDLFlBQVksQ0FBQyxRQUFRLENBQUMsTUFBTSxDQUFDO1lBQzlCLE9BQU87UUFDWCxJQUFJLENBQUMsT0FBTztZQUNSLE9BQU87UUFFWCxpREFBaUQ7UUFDakQsWUFBWSxDQUFDLFlBQVksQ0FBQyxPQUFPLENBQUMsT0FBTyxDQUFDLENBQUM7UUFDM0MsWUFBWSxDQUFDLE9BQU8sQ0FBQyxVQUFVLEdBQUcsSUFBSSxDQUFDO1FBRXZDLHlDQUF5QztRQUN6QyxZQUFZLENBQUMsT0FBTyxDQUFDLE9BQU8sR0FBRyxVQUFVLENBQUM7WUFDeEMsWUFBWSxDQUFDLE9BQU8sQ0FBQyxVQUFVLEdBQUcsS0FBSyxDQUFDO1FBQzFDLENBQUMsRUFBRSxHQUFHLENBQUMsQ0FBQztRQUVSLElBQUksVUFBVSxHQUFHLElBQUksQ0FBQztRQUV0Qiw0RUFBNEU7UUFDNUUsSUFBSSxHQUFHLENBQUMsTUFBTSxHQUFHLENBQUM7WUFBRSxVQUFVLEdBQUcsSUFBSSxDQUFDO1FBRXRDLElBQUksWUFBWSxLQUFLLGlCQUFpQixFQUFDLENBQUM7WUFDdEMsSUFBSSxFQUFFLEdBQUcsVUFBVSxDQUFDLE9BQU8sQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDO1lBQ2hDLElBQUksRUFBRSxHQUFHLFVBQVUsQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztZQUNoQyxJQUFJLGFBQWEsQ0FBQyxDQUFDLENBQUMsR0FBRyxVQUFVO2dCQUM3QixFQUFFLEdBQUcsVUFBVSxHQUFHLENBQUMsRUFBRSxHQUFHLEVBQUUsQ0FBQyxHQUFHLEVBQUUsQ0FBQztpQkFDaEMsSUFBSSxhQUFhLENBQUMsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxRQUFRLEdBQUcsV0FBVyxDQUFDO2dCQUNoRCxFQUFFLEdBQUcsRUFBRSxHQUFHLFVBQVUsR0FBRyxDQUFDLEVBQUUsR0FBRyxFQUFFLENBQUMsQ0FBQztpQkFDaEMsQ0FBQztnQkFDRixJQUFNLE9BQU8sR0FBRyxhQUFhLENBQUMsQ0FBQyxDQUFDLENBQUM7Z0JBQ2pDLEVBQUUsR0FBRyxPQUFPLEdBQUcsQ0FBQyxPQUFPLEdBQUcsRUFBRSxDQUFDLEdBQUcsVUFBVSxDQUFDO2dCQUMzQyxFQUFFLEdBQUcsT0FBTyxHQUFHLENBQUMsRUFBRSxHQUFHLE9BQU8sQ0FBQyxHQUFHLFVBQVUsQ0FBQztZQUMvQyxDQUFDO1lBQ0QsSUFBSSxDQUFDLEVBQUUsR0FBQyxFQUFFLENBQUMsR0FBRyxFQUFFLEVBQUUsQ0FBQztnQkFDakIsSUFBSSxVQUFVLFNBQWlCLENBQUM7Z0JBQ2hDLElBQUksTUFBQSxLQUFLLENBQUMsU0FBUyxtQ0FBSSxLQUFLO29CQUFFLFVBQVUsR0FBRyxDQUFDLElBQUksQ0FBQyxHQUFHLENBQUMsY0FBYyxDQUFDLENBQUMsQ0FBQyxFQUFDLGFBQWEsQ0FBQyxFQUFFLENBQUMsQ0FBQyxFQUFFLElBQUksQ0FBQyxHQUFHLENBQUMsY0FBYyxDQUFDLENBQUMsQ0FBQyxFQUFFLGFBQWEsQ0FBQyxFQUFFLENBQUMsQ0FBQyxDQUFDLENBQUE7O29CQUNySSxVQUFVLEdBQUcsQ0FBQyxhQUFhLENBQUMsRUFBRSxDQUFDLEVBQUUsYUFBYSxDQUFDLEVBQUUsQ0FBQyxDQUFDLENBQUM7Z0JBQ3pELElBQUksWUFBWSxLQUFLLGVBQWUsRUFBRSxDQUFDO29CQUNyQyxJQUFNLFVBQVUsR0FBRyxxQkFBcUIsQ0FBQyxVQUFVLENBQUMsQ0FBQztvQkFDckQsSUFBSSxDQUFDLENBQUMsQ0FBQyxPQUFPLENBQUMsVUFBVSxFQUFFLE9BQU8sQ0FBQzt3QkFBRSxVQUFVLENBQUMsVUFBVSxDQUFDLENBQUM7Z0JBQzlELENBQUM7Z0JBQ0QsVUFBVSxDQUFDLFVBQVUsQ0FBQyxDQUFDO1lBQ3pCLENBQUM7UUFDSCxDQUFDO1FBRUQsSUFBSSxZQUFZLEtBQUssZUFBZSxFQUFFLENBQUM7WUFDckMsSUFBTSxVQUFVLEdBQUcsT0FBTyxDQUFDLEdBQUcsQ0FBQyxVQUFDLE1BQXVCLEVBQUUsSUFBWSxFQUFFLFVBQThCOztnQkFDbkcsSUFBSSxFQUFFLEdBQUcsVUFBVSxDQUFDLE1BQU0sQ0FBQyxDQUFDLENBQUMsRUFBRSxJQUFJLENBQUMsQ0FBQztnQkFDckMsSUFBSSxFQUFFLEdBQUcsVUFBVSxDQUFDLE1BQU0sQ0FBQyxDQUFDLENBQUMsRUFBRSxJQUFJLENBQUMsQ0FBQztnQkFFckMsSUFBSSxhQUFhLENBQUMsQ0FBQyxDQUFDLEdBQUcsU0FBUztvQkFDNUIsRUFBRSxHQUFHLFVBQVUsR0FBRyxDQUFDLEVBQUUsR0FBRyxFQUFFLENBQUMsR0FBRyxFQUFFLENBQUM7cUJBRWhDLElBQUksYUFBYSxDQUFDLENBQUMsQ0FBQyxHQUFHLENBQUMsU0FBUyxHQUFHLFlBQVksQ0FBQztvQkFDbEQsRUFBRSxHQUFHLEVBQUUsR0FBRyxVQUFVLEdBQUcsQ0FBQyxFQUFFLEdBQUcsRUFBRSxDQUFDLENBQUM7cUJBRWhDLENBQUM7b0JBQ0YsSUFBTSxPQUFPLEdBQUcsYUFBYSxDQUFDLENBQUMsQ0FBQyxDQUFDO29CQUNqQyxFQUFFLEdBQUcsT0FBTyxHQUFHLENBQUMsT0FBTyxHQUFHLEVBQUUsQ0FBQyxHQUFHLFVBQVUsQ0FBQztvQkFDM0MsRUFBRSxHQUFHLE9BQU8sR0FBRyxDQUFDLEVBQUUsR0FBRyxPQUFPLENBQUMsR0FBRyxVQUFVLENBQUM7Z0JBQy9DLENBQUM7Z0JBRUQsSUFBSSxJQUFJLENBQUMsR0FBRyxDQUFDLEVBQUUsR0FBQyxFQUFFLENBQUMsR0FBRyxFQUFFLEVBQUUsQ0FBQztvQkFDekIsSUFBSSxNQUFBLEtBQUssQ0FBQyxTQUFTLG1DQUFJLEtBQUs7d0JBQUUsT0FBTyxDQUFDLElBQUksQ0FBQyxHQUFHLENBQUMsY0FBYyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsQ0FBQyxFQUFDLGFBQWEsQ0FBQyxFQUFFLEVBQUUsSUFBSSxDQUFDLENBQUMsRUFBRSxJQUFJLENBQUMsR0FBRyxDQUFDLGNBQWMsQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsRUFBRSxhQUFhLENBQUMsRUFBRSxFQUFFLElBQUksQ0FBQyxDQUFDLENBQUMsQ0FBQztvQkFDN0osT0FBTyxDQUFDLGFBQWEsQ0FBQyxFQUFFLEVBQUUsSUFBSSxDQUFDLEVBQUUsYUFBYSxDQUFDLEVBQUUsRUFBRSxJQUFJLENBQUMsQ0FBQyxDQUFDO2dCQUM1RCxDQUFDO2dCQUNELE9BQU8sTUFBTSxDQUFDO1lBQ2hCLENBQUMsQ0FBQyxDQUFDO1lBQ0gsSUFBSSxDQUFDLENBQUMsQ0FBQyxPQUFPLENBQUMsVUFBVSxFQUFFLE9BQU8sQ0FBQyxFQUFFLENBQUM7Z0JBQ3BDLGlFQUFpRTtnQkFDakUsVUFBVSxDQUFDLFVBQVUsQ0FBQyxDQUFDO1lBQ3pCLENBQUM7UUFDSCxDQUFDO0lBQ0wsQ0FBQztJQUVILFNBQVMsZUFBZSxDQUFDLEdBQVE7UUFDL0IsSUFBSSxDQUFDLGFBQWEsQ0FBQyxPQUFPO1lBQ3hCLHFCQUFxQixDQUFDLGNBQU0sT0FBQSxjQUFjLENBQUMsR0FBRyxDQUFDLEVBQW5CLENBQW1CLENBQUMsQ0FBQztRQUNuRCxhQUFhLENBQUMsT0FBTyxHQUFHLElBQUksQ0FBQztJQUMvQixDQUFDO0lBRUQsU0FBUyxjQUFjLENBQUMsR0FBUTs7UUFDOUIsYUFBYSxDQUFDLE9BQU8sR0FBRyxLQUFLLENBQUM7UUFDOUIsSUFBSSxNQUFNLENBQUMsT0FBTyxJQUFJLElBQUk7WUFDeEIsT0FBTztRQUNULElBQU0sRUFBRSxHQUFHLE1BQU0sQ0FBQyxPQUFRLENBQUMsY0FBYyxFQUFFLENBQUM7UUFDNUMsRUFBRSxDQUFDLENBQUMsR0FBRyxHQUFHLENBQUMsT0FBTyxDQUFDO1FBQ25CLEVBQUUsQ0FBQyxDQUFDLEdBQUcsR0FBRyxDQUFDLE9BQU8sQ0FBQztRQUNuQixJQUFNLFdBQVcsR0FBRyxFQUFFLENBQUMsZUFBZSxDQUFDLE1BQU0sQ0FBQyxPQUFRLENBQUMsWUFBWSxFQUFFLENBQUMsT0FBTyxFQUFFLENBQUMsQ0FBQztRQUVqRixJQUFJLFNBQVMsS0FBSyxLQUFLLEVBQUUsQ0FBQztZQUN4QixJQUFNLEVBQUUsR0FBRyxhQUFhLENBQUMsQ0FBQyxDQUFDLEdBQUcsV0FBVyxDQUFDLENBQUMsQ0FBQztZQUM1QyxJQUFNLE1BQU0sR0FBRyxVQUFVLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7WUFDdEMsSUFBTSxNQUFNLEdBQUcsVUFBVSxDQUFDLE9BQU8sQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFBO1lBQ3JDLElBQU0sSUFBSSxHQUFHLGFBQWEsQ0FBQyxNQUFNLEdBQUcsRUFBRSxDQUFDLENBQUM7WUFDeEMsSUFBTSxJQUFJLEdBQUcsYUFBYSxDQUFDLE1BQU0sR0FBRyxFQUFFLENBQUMsQ0FBQztZQUN4QyxJQUNFLENBQUMsS0FBSyxDQUFDLElBQUksS0FBSyxTQUFTLElBQUksSUFBSSxHQUFJLEtBQUssQ0FBQyxJQUFJLENBQUM7Z0JBQ2hELENBQUMsS0FBSyxDQUFDLElBQUksS0FBSyxTQUFTLElBQUksSUFBSSxHQUFHLEtBQUssQ0FBQyxJQUFJLENBQUM7Z0JBQy9DLFVBQVUsQ0FBQyxDQUFDLElBQUksRUFBRSxJQUFJLENBQUMsQ0FBQyxDQUFDO1lBRTNCLElBQU0sU0FBUyxHQUFHLFVBQUMsTUFBdUIsRUFBRSxJQUFZLEVBQUUsVUFBOEI7Z0JBQ3RGLElBQU0sRUFBRSxHQUFHLGFBQWEsQ0FBQyxhQUFhLENBQUMsQ0FBQyxDQUFDLEVBQUUsSUFBSSxDQUFDLEdBQUcsYUFBYSxDQUFDLFdBQVcsQ0FBQyxDQUFDLEVBQUUsSUFBSSxDQUFDLENBQUM7Z0JBQ3RGLHNDQUFzQztnQkFDdEMsSUFBTSxPQUFPLEdBQXVCLFdBQVcsQ0FBUyxLQUFLLENBQUMsSUFBSSxFQUFFLElBQUksQ0FBQyxDQUFDO2dCQUMxRSxJQUFNLE9BQU8sR0FBdUIsV0FBVyxDQUFTLEtBQUssQ0FBQyxJQUFJLEVBQUUsSUFBSSxDQUFDLENBQUM7Z0JBQzFFLElBQ0UsQ0FBQyxPQUFPLEtBQUssU0FBUyxJQUFJLE1BQU0sQ0FBQyxDQUFDLENBQUMsR0FBRyxFQUFFLEdBQUksT0FBTyxDQUFDO29CQUNwRCxDQUFDLE9BQU8sS0FBSyxTQUFTLElBQUksTUFBTSxDQUFDLENBQUMsQ0FBQyxHQUFHLEVBQUUsR0FBRyxPQUFPLENBQUMsRUFBRSxDQUFDO29CQUNwRCxVQUFVLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxNQUFNLENBQUMsQ0FBQyxDQUFDLEdBQUcsRUFBRSxFQUFFLE1BQU0sQ0FBQyxDQUFDLENBQUMsR0FBRyxFQUFFLENBQUMsQ0FBQztvQkFDcEQsT0FBTyxJQUFJLENBQUM7Z0JBQ2QsQ0FBQztnQkFDSCxPQUFPLEtBQUssQ0FBQztZQUNmLENBQUMsQ0FBQTtZQUNELGNBQWMsQ0FBQyxTQUFTLENBQUMsQ0FBQztRQUM1QixDQUFDO1FBQ0QsZ0JBQWdCLENBQUMsQ0FBQyxXQUFXLENBQUMsQ0FBQyxFQUFFLFdBQVcsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDO1FBQ2pELDJDQUEyQztRQUMzQyxJQUFJLE9BQStCLENBQUM7UUFDcEMsSUFBSSxNQUFBLEtBQUssQ0FBQyxTQUFTLG1DQUFJLEtBQUs7WUFBRSxPQUFPLEdBQUcsd0JBQXdCLENBQUMsV0FBVyxDQUFDLENBQUM7O1lBQ3pFLE9BQU8sR0FBRyxXQUFXLENBQUM7UUFDM0Isb0JBQW9CLENBQUMsQ0FBQyxPQUFPLENBQUMsQ0FBQyxFQUFFLE9BQU8sQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDO1FBQzdDLElBQUksUUFBUSxDQUFDLE9BQU8sQ0FBQyxJQUFJLEdBQUcsQ0FBQztZQUMzQixRQUFRLENBQUMsT0FBTyxDQUFDLE9BQU8sQ0FBQyxVQUFDLENBQUMsSUFBSyxPQUFBLENBQUMsQ0FBQyxDQUFDLE1BQU0sS0FBSyxTQUFTLENBQUEsQ0FBQyxDQUFDLENBQUMsQ0FBQyxNQUFNLENBQUMsYUFBYSxDQUFDLENBQUMsQ0FBQyxhQUFhLENBQUMsQ0FBQyxDQUFDLE9BQU8sQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLFdBQVcsQ0FBQyxDQUFDLENBQUMsRUFBRSxhQUFhLENBQUMsQ0FBQyxDQUFDLGFBQWEsQ0FBQyxDQUFDLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsV0FBVyxDQUFDLENBQUMsRUFBRSxDQUFDLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsSUFBSSxDQUFDLEVBQXpLLENBQXlLLENBQUMsQ0FBQztJQUMvTSxDQUFDO0lBRUQsU0FBUyxlQUFlLENBQUMsR0FBUTs7UUFDL0IsSUFBSSxNQUFNLENBQUMsT0FBTyxJQUFJLElBQUk7WUFDeEIsT0FBTztRQUVQLElBQU0sRUFBRSxHQUFHLE1BQU0sQ0FBQyxPQUFRLENBQUMsY0FBYyxFQUFFLENBQUM7UUFDNUMsRUFBRSxDQUFDLENBQUMsR0FBRyxHQUFHLENBQUMsT0FBTyxDQUFDO1FBQ25CLEVBQUUsQ0FBQyxDQUFDLEdBQUcsR0FBRyxDQUFDLE9BQU8sQ0FBQztRQUNuQixJQUFNLFdBQVcsR0FBRyxFQUFFLENBQUMsZUFBZSxDQUFDLE1BQU0sQ0FBQyxPQUFRLENBQUMsWUFBWSxFQUFFLENBQUMsT0FBTyxFQUFFLENBQUMsQ0FBQTtRQUNoRixhQUFhLENBQUMsQ0FBQyxXQUFXLENBQUMsQ0FBQyxFQUFFLFdBQVcsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDO1FBQzlDLElBQUksWUFBWSxDQUFDLFFBQVEsQ0FBQyxNQUFNLENBQUMsSUFBSSxDQUFDLEtBQUssQ0FBQyxJQUFJLEtBQUssU0FBUyxJQUFJLEtBQUssQ0FBQyxJQUFJLENBQUM7WUFDekUsWUFBWSxDQUFDLFlBQVksQ0FBQyxDQUFDO1FBQy9CLElBQUksWUFBWSxLQUFLLEtBQUssSUFBSSxDQUFDLEtBQUssQ0FBQyxHQUFHLEtBQUssU0FBUyxJQUFJLEtBQUssQ0FBQyxHQUFHLENBQUMsRUFBRSxDQUFDO1lBQ25FLFlBQVksQ0FBQyxLQUFLLENBQUMsQ0FBQztZQUNwQixhQUFhLENBQUMsVUFBVSxDQUFDLENBQUM7UUFDOUIsQ0FBQztRQUVELG9GQUFvRjtRQUNwRiwyQ0FBMkM7UUFDM0MsSUFBSSxPQUErQixDQUFDO1FBQ3BDLElBQUksTUFBQSxLQUFLLENBQUMsU0FBUyxtQ0FBSSxLQUFLO1lBQUUsT0FBTyxHQUFHLHdCQUF3QixDQUFDLFdBQVcsQ0FBQyxDQUFDOztZQUN6RSxPQUFPLEdBQUcsV0FBVyxDQUFDO1FBQzNCLElBQUksWUFBWSxLQUFLLFFBQVEsSUFBSSxLQUFLLENBQUMsUUFBUSxLQUFLLFNBQVM7WUFDM0QsS0FBSyxDQUFDLFFBQVEsQ0FDWixhQUFhLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQyxFQUN4Qix5QkFBSSxzQkFBTyxDQUFDLE1BQU0sRUFBRSxVQUFFLEdBQUcsQ0FBQyxVQUFBLElBQUksSUFBSSxPQUFBLGFBQWEsQ0FBQyxPQUFPLENBQUMsQ0FBQyxFQUFFLElBQUksQ0FBQyxFQUE5QixDQUE4QixDQUFDLEVBQ2pFO2dCQUNBLFVBQVUsRUFBRSxhQUFzRDtnQkFDbEUsVUFBVSxFQUFFLGFBQXdEO2FBQ25FLENBQUMsQ0FBQztRQUNQLElBQUksUUFBUSxDQUFDLE9BQU8sQ0FBQyxJQUFJLEdBQUcsQ0FBQyxJQUFJLFlBQVksS0FBSyxRQUFRO1lBQ3hELFFBQVEsQ0FBQyxPQUFPLENBQUMsT0FBTyxDQUFDLFVBQUMsQ0FBQyxJQUFLLE9BQUEsQ0FBQyxDQUFDLENBQUMsT0FBTyxLQUFLLFNBQVMsQ0FBQSxDQUFDLENBQUMsQ0FBQyxDQUFDLE9BQU8sQ0FBQyxhQUFhLENBQUMsQ0FBQyxDQUFDLGFBQWEsQ0FBQyxDQUFDLENBQUMsT0FBTyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsV0FBVyxDQUFDLENBQUMsQ0FBQyxFQUFFLGFBQWEsQ0FBQyxDQUFDLENBQUMsYUFBYSxDQUFDLENBQUMsQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxXQUFXLENBQUMsQ0FBQyxFQUFFLENBQUMsQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUMsRUFBM0ssQ0FBMkssQ0FBQyxDQUFDO0lBQ25OLENBQUM7SUFFRCxTQUFTLGFBQWE7UUFDcEIsSUFBSSxZQUFZLEtBQUssS0FBSyxJQUFJLENBQUMsS0FBSyxDQUFDLEdBQUcsS0FBSyxTQUFTLElBQUksS0FBSyxDQUFDLEdBQUcsQ0FBQztZQUNoRSxhQUFhLENBQUMsTUFBTSxDQUFDLENBQUM7UUFDMUIsSUFBSSxTQUFTLENBQUMsUUFBUSxDQUFDLE1BQU0sQ0FBQyxFQUFFLENBQUM7WUFFN0IsSUFBSSxDQUFDLElBQUksQ0FBQyxHQUFHLENBQUMsYUFBYSxDQUFDLENBQUMsQ0FBQyxHQUFHLFVBQVUsQ0FBQyxDQUFDLENBQUMsQ0FBQyxHQUFHLEVBQUUsQ0FBQyxJQUFJLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxhQUFhLENBQUMsQ0FBQyxDQUFDLEdBQUcsVUFBVSxDQUFDLENBQUMsQ0FBQyxDQUFDLEdBQUcsRUFBRSxDQUFDLEVBQUUsQ0FBQztnQkFDekcsWUFBWSxDQUFDLE1BQU0sQ0FBQyxDQUFDO2dCQUNyQixPQUFPO1lBQ1gsQ0FBQztZQUVELElBQUksU0FBUyxLQUFLLGlCQUFpQixFQUFDLENBQUM7Z0JBQ25DLElBQU0sRUFBRSxHQUFHLElBQUksQ0FBQyxHQUFHLENBQUMsYUFBYSxDQUFDLGFBQWEsQ0FBQyxDQUFDLENBQUMsQ0FBQyxFQUFFLGFBQWEsQ0FBQyxVQUFVLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDO2dCQUNuRixJQUFNLEVBQUUsR0FBRyxJQUFJLENBQUMsR0FBRyxDQUFDLGFBQWEsQ0FBQyxhQUFhLENBQUMsQ0FBQyxDQUFDLENBQUMsRUFBRSxhQUFhLENBQUMsVUFBVSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztnQkFDbkYsSUFBTSxVQUFVLEdBQXFCLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDLEVBQUUsRUFBRSxDQUFDLEVBQUUsSUFBSSxDQUFDLEdBQUcsQ0FBQyxPQUFPLENBQUMsQ0FBQyxDQUFDLEVBQUUsRUFBRSxDQUFDLENBQUMsQ0FBQztnQkFDMUYsSUFBSSxZQUFZLEtBQUssZUFBZSxFQUFFLENBQUM7b0JBQ3JDLElBQU0sVUFBVSxHQUFHLHFCQUFxQixDQUFDLFVBQVUsQ0FBQyxDQUFDO29CQUNyRCxJQUFJLENBQUMsQ0FBQyxDQUFDLE9BQU8sQ0FBQyxVQUFVLEVBQUUsT0FBTyxDQUFDO3dCQUFFLFVBQVUsQ0FBQyxVQUFVLENBQUMsQ0FBQztnQkFDOUQsQ0FBQztnQkFDRCxVQUFVLENBQUMsVUFBVSxDQUFDLENBQUM7WUFDekIsQ0FBQztZQUVELElBQUksU0FBUyxLQUFLLGVBQWUsRUFBRSxDQUFDO2dCQUNsQyxJQUFNLFVBQVUsR0FBRyxPQUFPLENBQUMsR0FBRyxDQUFDLFVBQUMsTUFBdUIsRUFBRSxJQUFZLEVBQUUsVUFBOEI7b0JBQ25HLElBQU0sRUFBRSxHQUFHLElBQUksQ0FBQyxHQUFHLENBQUMsYUFBYSxDQUFDLGFBQWEsQ0FBQyxDQUFDLENBQUMsRUFBRSxJQUFJLENBQUMsRUFBRSxhQUFhLENBQUMsVUFBVSxDQUFDLENBQUMsQ0FBQyxFQUFFLElBQUksQ0FBQyxDQUFDLENBQUM7b0JBQy9GLElBQU0sRUFBRSxHQUFHLElBQUksQ0FBQyxHQUFHLENBQUMsYUFBYSxDQUFDLGFBQWEsQ0FBQyxDQUFDLENBQUMsRUFBRSxJQUFJLENBQUMsRUFBRSxhQUFhLENBQUMsVUFBVSxDQUFDLENBQUMsQ0FBQyxFQUFFLElBQUksQ0FBQyxDQUFDLENBQUM7b0JBQy9GLE9BQU8sQ0FBQyxJQUFJLENBQUMsR0FBRyxDQUFDLE1BQU0sQ0FBQyxDQUFDLENBQUMsRUFBRSxFQUFFLENBQUMsRUFBRSxJQUFJLENBQUMsR0FBRyxDQUFDLE1BQU0sQ0FBQyxDQUFDLENBQUMsRUFBRSxFQUFFLENBQUMsQ0FBQyxDQUFDO2dCQUM1RCxDQUFDLENBQUMsQ0FBQztnQkFDSCxJQUFJLENBQUMsQ0FBQyxDQUFDLE9BQU8sQ0FBQyxVQUFVLEVBQUUsT0FBTyxDQUFDLEVBQUUsQ0FBQztvQkFDcEMsaUVBQWlFO29CQUNqRSxVQUFVLENBQUMsVUFBVSxDQUFDLENBQUM7Z0JBQ3pCLENBQUM7WUFDSCxDQUFDO1FBQ0wsQ0FBQztRQUNELFlBQVksQ0FBQyxNQUFNLENBQUMsQ0FBQztRQUVyQixJQUFJLFFBQVEsQ0FBQyxPQUFPLENBQUMsSUFBSSxHQUFHLENBQUMsSUFBSSxZQUFZLEtBQUssUUFBUTtZQUN4RCxRQUFRLENBQUMsT0FBTyxDQUFDLE9BQU8sQ0FBQyxVQUFDLENBQUMsSUFBSyxPQUFBLENBQUMsQ0FBQyxDQUFDLFNBQVMsS0FBSyxTQUFTLENBQUEsQ0FBQyxDQUFDLENBQUMsQ0FBQyxTQUFTLENBQUMsYUFBYSxDQUFDLENBQUMsQ0FBQyxhQUFhLENBQUMsQ0FBQyxDQUFDLGlCQUFpQixDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxhQUFhLENBQUMsQ0FBQyxDQUFDLENBQUMsRUFBRSxhQUFhLENBQUMsQ0FBQyxDQUFDLGFBQWEsQ0FBQyxDQUFDLENBQUMsaUJBQWlCLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLGFBQWEsQ0FBQyxDQUFDLENBQUMsRUFBRSxDQUFDLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsSUFBSSxDQUFDLEVBQTNNLENBQTJNLENBQUMsQ0FBQztJQUNqUCxDQUFDO0lBRUQsU0FBUyxjQUFjLENBQUMsQ0FBTTtRQUMxQixVQUFVLENBQUMsS0FBSyxDQUFDLENBQUM7UUFDbEIsSUFBSSxTQUFTLEtBQUssS0FBSztZQUNuQixZQUFZLENBQUMsTUFBTSxDQUFDLENBQUM7UUFFekIsSUFBSSxRQUFRLENBQUMsT0FBTyxDQUFDLElBQUksR0FBRyxDQUFDLElBQUksWUFBWSxLQUFLLFFBQVE7WUFDeEQsUUFBUSxDQUFDLE9BQU8sQ0FBQyxPQUFPLENBQUMsVUFBQyxDQUFDLElBQUssT0FBQSxDQUFDLENBQUMsQ0FBQyxXQUFXLEtBQUssU0FBUyxDQUFBLENBQUMsQ0FBQyxDQUFDLENBQUMsV0FBVyxDQUFDLGFBQWEsQ0FBQyxDQUFDLENBQUMsYUFBYSxDQUFDLENBQUMsQ0FBQyxpQkFBaUIsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsYUFBYSxDQUFDLENBQUMsQ0FBQyxDQUFDLEVBQUUsYUFBYSxDQUFDLENBQUMsQ0FBQyxhQUFhLENBQUMsQ0FBQyxDQUFDLGlCQUFpQixDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxhQUFhLENBQUMsQ0FBQyxDQUFDLEVBQUUsQ0FBQyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLElBQUksQ0FBQyxFQUEvTSxDQUErTSxDQUFDLENBQUM7SUFFdlAsQ0FBQztJQUVELFNBQVMsYUFBYSxDQUFDLENBQU07UUFDekIsVUFBVSxDQUFDLElBQUksQ0FBQyxDQUFDO0lBQ3JCLENBQUM7SUFFRCxTQUFTLGFBQWEsQ0FBQyxDQUFrQjtRQUN2QyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsS0FBSyxPQUFPLENBQUMsQ0FBQyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsQ0FBQyxLQUFLLE9BQU8sQ0FBQyxDQUFDLENBQUM7WUFDNUMsT0FBTztRQUVULElBQUksQ0FBQyxDQUFDLENBQUMsQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLENBQUM7WUFDYixVQUFVLENBQUMsQ0FBQyxDQUFDLENBQUM7O1lBRWQsVUFBVSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxFQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7SUFDNUIsQ0FBQztJQUVELFNBQVMsYUFBYSxDQUFDLENBQW9CO1FBQ3pDLElBQU0sZUFBZSxHQUFHLFVBQUMsTUFBdUIsRUFBRSxJQUFZLEVBQUUsVUFBOEI7WUFDNUYsSUFBSSxDQUFDLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLEtBQUssTUFBTSxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUMsQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsS0FBSyxNQUFNLENBQUMsQ0FBQyxDQUFDO2dCQUN0RCxPQUFPLEtBQUssQ0FBQztZQUVmLElBQUksQ0FBQyxDQUFDLENBQUMsQ0FBQyxHQUFHLENBQUMsQ0FBQyxDQUFDLENBQUM7Z0JBQ2IsVUFBVSxDQUFDLElBQUksQ0FBQyxHQUFHLENBQUMsQ0FBQyxJQUFJLENBQUMsQ0FBQzs7Z0JBRTNCLFVBQVUsQ0FBQyxJQUFJLENBQUMsR0FBRyxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsRUFBQyxDQUFDLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztZQUM3QyxPQUFPLElBQUksQ0FBQztRQUNkLENBQUMsQ0FBQTtRQUNELGNBQWMsQ0FBQyxlQUFlLENBQUMsQ0FBQztJQUNsQyxDQUFDO0lBRUQsT0FBTyxDQUNMLG9CQUFDLDZCQUFjLElBQ2IsT0FBTyxFQUFFLE9BQU8sRUFDaEIsYUFBYSxFQUFFLGFBQWEsRUFDNUIsaUJBQWlCLEVBQUUsaUJBQWlCLEVBQ3BDLE9BQU8sRUFBRSxPQUFPLEVBQ2hCLFdBQVcsRUFBRSxZQUFZLEVBQ3pCLE9BQU8sRUFBRSxPQUFPLEVBQ2hCLFVBQVUsRUFBRSxVQUFVLEVBQ3RCLElBQUksRUFBRSxJQUFJLEVBQ1YsUUFBUSxFQUFFLFFBQVEsRUFDbEIsaUJBQWlCLEVBQUUsWUFBWSxFQUMvQixpQkFBaUIsRUFBRSxZQUFZLEVBQy9CLFVBQVUsRUFBRSxVQUFVLEVBQ3RCLFVBQVUsRUFBRSxVQUFVLEVBQ3RCLGFBQWEsRUFBRSxhQUFhLEVBQzVCLGFBQWEsRUFBRSxhQUFhLEVBQzVCLFVBQVUsRUFBRSxhQUFhLEVBQ3pCLFVBQVUsRUFBRSxhQUFhLEVBQ3pCLE9BQU8sRUFBRSxPQUFPLEVBQ2hCLFVBQVUsRUFBRSxPQUFPLEVBQ25CLFVBQVUsRUFBRSxPQUFPLEVBQ25CLFNBQVMsRUFBRSxTQUFTLEVBQ3BCLGNBQWMsRUFBRSxjQUFjLEVBQzlCLFlBQVksRUFBRSxZQUFZLEVBQzFCLFlBQVksRUFBRSxZQUFZO1FBRXhCLDZCQUFLLEVBQUUsRUFBRSxJQUFJLEVBQUUsS0FBSyxFQUFFLEVBQUUsTUFBTSxFQUFFLEtBQUssQ0FBQyxNQUFNLEVBQUUsS0FBSyxFQUFFLEtBQUssQ0FBQyxLQUFLLEVBQUUsUUFBUSxFQUFFLFVBQVUsRUFBRTtZQUNwRiw2QkFBSyxLQUFLLEVBQUUsRUFBRSxNQUFNLEVBQUUsU0FBUyxFQUFFLEtBQUssRUFBRSxRQUFRLEVBQUUsUUFBUSxFQUFFLFVBQVUsRUFBRSxNQUFNLEVBQUUsVUFBVSxFQUFFLEVBQ3hGLE9BQU8sRUFBRSxnQkFBZ0IsRUFBRSxXQUFXLEVBQUUsZUFBZSxFQUFFLFdBQVcsRUFBRSxlQUFlLEVBQUUsU0FBUyxFQUFFLGFBQWEsRUFBRSxZQUFZLEVBQUUsY0FBYyxFQUFFLFlBQVksRUFBRSxhQUFhO2dCQUMxSyw2QkFBSyxHQUFHLEVBQUUsTUFBTSxFQUFFLEtBQUssRUFBRSxRQUFRLEdBQUcsQ0FBQyxDQUFBLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLFFBQVEsRUFBRSxNQUFNLEVBQUUsU0FBUyxHQUFHLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxTQUFTLEVBQ3pGLEtBQUssRUFBRSxRQUFRLEVBQUUsT0FBTyxFQUFFLGNBQU8sUUFBUSxHQUFHLENBQUMsQ0FBQSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxRQUFRLGNBQUksU0FBUyxHQUFHLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxTQUFTLENBQUU7b0JBQ3pGLEtBQUssQ0FBQyxVQUFVLEtBQUssU0FBUyxJQUFJLEtBQUssQ0FBQyxVQUFVLENBQUMsQ0FBQyxDQUFDLDhCQUFPLE1BQU0sRUFBQyxPQUFPLEVBQUMsQ0FBQyxFQUFFLFlBQUssVUFBVSxjQUFJLFNBQVMsZ0JBQU0sUUFBUSxHQUFFLFdBQVcsZ0JBQU0sU0FBUyxHQUFHLFlBQVksZ0JBQU0sVUFBVSxPQUFJLEdBQUksQ0FBQyxDQUFDLENBQUMsSUFBSTtvQkFDbE0sS0FBSyxDQUFDLFNBQVMsS0FBSyxNQUFNLElBQUksS0FBSyxDQUFDLFNBQVMsS0FBSyxTQUFTLENBQUMsQ0FBQzt3QkFDL0Qsb0JBQUMsa0JBQVEsSUFBQyxLQUFLLEVBQUUsS0FBSyxDQUFDLE1BQU0sRUFBRSxZQUFZLEVBQUUsWUFBWSxFQUFFLFVBQVUsRUFBRSxVQUFVLEVBQUUsV0FBVyxFQUFFLFdBQVcsRUFBRSxLQUFLLEVBQUUsUUFBUSxFQUFFLE1BQU0sRUFBRSxTQUFTLEVBQUUsU0FBUyxFQUFFLGVBQWUsRUFDekssVUFBVSxFQUFFLFlBQVksRUFBRSxnQkFBZ0IsRUFBRSxDQUFDLFFBQVEsQ0FBQyxDQUFDLENBQUMsRUFBRyxpQkFBaUIsRUFBRSxDQUFDLFFBQVEsQ0FBQyxDQUFDLENBQUMsRUFBRSxRQUFRLEVBQUUsS0FBSyxDQUFDLGtCQUFrQixHQUFJLENBQUMsQ0FBQzt3QkFDcEksS0FBSyxDQUFDLFNBQVMsS0FBSyxPQUFPLENBQUMsQ0FBQyxDQUFDLG9CQUFDLG9CQUFVLElBQUMsWUFBWSxFQUFFLFlBQVksRUFBRSxVQUFVLEVBQUUsVUFBVSxFQUFFLFdBQVcsRUFBRSxXQUFXLEVBQUUsS0FBSyxFQUFFLFFBQVEsRUFBRSxNQUFNLEVBQUUsU0FBUyxFQUFFLFNBQVMsRUFBRSxlQUFlLEVBQUUsVUFBVSxFQUFFLFlBQVksRUFDaE4sS0FBSyxFQUFFLEtBQUssQ0FBQyxNQUFNLEVBQUUsZ0JBQWdCLEVBQUUsQ0FBQyxRQUFRLENBQUMsQ0FBQyxDQUFDLEVBQUcsaUJBQWlCLEVBQUUsQ0FBQyxRQUFRLENBQUMsQ0FBQyxDQUFDLEdBQUcsQ0FBRSxDQUFDOzRCQUM3RixvQkFBQyxpQkFBTyxJQUFDLFNBQVMsRUFBRSxTQUFTLEVBQUUsUUFBUSxFQUFFLEtBQUssQ0FBQyxRQUFRLEVBQUUsS0FBSyxFQUFFLEtBQUssQ0FBQyxNQUFNLEVBQUUsWUFBWSxFQUFFLFlBQVksRUFBRSxVQUFVLEVBQUUsVUFBVSxFQUFFLFdBQVcsRUFBRSxXQUFXLEVBQUUsS0FBSyxFQUFFLFFBQVEsRUFDekssTUFBTSxFQUFFLFNBQVMsRUFBRSxTQUFTLEVBQUUsZUFBZSxFQUFFLFVBQVUsRUFBRSxZQUFZLEVBQUUsZ0JBQWdCLEVBQUUsQ0FBQyxRQUFRLENBQUMsQ0FBQyxDQUFDLEVBQUcsaUJBQWlCLEVBQUUsQ0FBQyxRQUFRLENBQUMsQ0FBQyxDQUFDLEdBQUk7b0JBQzVJLENBQUMsTUFBQSxLQUFLLENBQUMsU0FBUyxtQ0FBSSxLQUFLLENBQUMsQ0FBQyxDQUFDLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUNuQzt3QkFDRyxRQUFRLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQ2Isb0JBQUMsb0JBQVUsSUFDVCxXQUFXLEVBQUUsV0FBVyxFQUN4QixRQUFRLEVBQUUsS0FBSyxDQUFDLFFBQVEsRUFDeEIsS0FBSyxFQUFFLFdBQVcsQ0FBUyxLQUFLLENBQUMsTUFBTSxFQUFFLENBQUMsQ0FBQyxFQUMzQyxTQUFTLEVBQUUsU0FBUyxFQUNwQixVQUFVLEVBQUUsVUFBVSxFQUN0QixZQUFZLEVBQUUsWUFBWSxFQUMxQixLQUFLLEVBQUUsUUFBUSxFQUNmLE1BQU0sRUFBRSxTQUFTLEVBQ2pCLFlBQVksRUFBRSxtQkFBbUIsRUFDakMsZUFBZSxFQUFFLGdCQUFnQixFQUNqQyxJQUFJLEVBQUUsTUFBTSxFQUNaLEtBQUssRUFBRSxnQkFBZ0IsRUFDdkIsT0FBTyxFQUFFLGFBQWEsRUFDdEIsU0FBUyxFQUFFLEtBQUssQ0FBQyxnQkFBZ0IsS0FBSyxTQUFTLENBQUMsQ0FBQyxDQUFDLElBQUksQ0FBQyxDQUFDLENBQUMsS0FBSyxDQUFDLGdCQUFnQixHQUMvRSxDQUNILENBQUMsQ0FBQyxDQUFDLElBQUk7d0JBRVAsUUFBUSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUNiLG9CQUFDLG9CQUFVLElBQ1QsV0FBVyxFQUFFLFdBQVcsRUFDeEIsUUFBUSxFQUFFLEtBQUssQ0FBQyxRQUFRLEVBQ3hCLEtBQUssRUFBRSxXQUFXLENBQVMsS0FBSyxDQUFDLE1BQU0sRUFBRSxDQUFDLENBQUMsRUFDM0MsU0FBUyxFQUFFLFNBQVMsRUFDcEIsVUFBVSxFQUFFLFVBQVUsRUFDdEIsWUFBWSxFQUFFLFlBQVksRUFDMUIsS0FBSyxFQUFFLFFBQVEsRUFDZixNQUFNLEVBQUUsU0FBUyxFQUNqQixZQUFZLEVBQUUsb0JBQW9CLEVBQ2xDLGVBQWUsRUFBRSxnQkFBZ0IsRUFDakMsSUFBSSxFQUFFLE9BQU8sRUFDYixLQUFLLEVBQUUsaUJBQWlCLEVBQ3hCLE9BQU8sRUFBRSxhQUFhLEVBQ3RCLFNBQVMsRUFBRSxLQUFLLENBQUMsZ0JBQWdCLEtBQUssU0FBUyxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLEtBQUssQ0FBQyxnQkFBZ0IsR0FDL0UsQ0FDSCxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQ1AsQ0FDRjtvQkFDTDt3QkFDSSxrQ0FBVSxFQUFFLEVBQUUsS0FBSyxHQUFHLElBQUk7NEJBQ3RCLDhCQUFNLE1BQU0sRUFBRSxNQUFNLEVBQUUsSUFBSSxFQUFFLE1BQU0sRUFBRSxDQUFDLEVBQUUsYUFBTSxVQUFVLGNBQUksU0FBUyxHQUFHLENBQUMsaUJBQU8sUUFBUSxHQUFHLFdBQVcsR0FBRyxDQUFDLGdCQUFNLFNBQVMsR0FBRyxZQUFZLGdCQUFNLFVBQVUsT0FBSSxHQUFJLENBQ3hKLENBQ1I7b0JBRVAsMkJBQUcsUUFBUSxFQUFFLFVBQVUsR0FBRyxJQUFJLEdBQUcsR0FBRzt3QkFDaEMsS0FBSyxDQUFDLFFBQVEsQ0FBQyxHQUFHLENBQUMsS0FBSyxDQUFDLFFBQVEsRUFBRSxVQUFDLE9BQU87NEJBQ2xDLElBQUksQ0FBQyxLQUFLLENBQUMsY0FBYyxDQUFDLE9BQU8sQ0FBQztnQ0FDOUIsT0FBTyxJQUFJLENBQUM7NEJBQ2hCLElBQUssT0FBbUMsQ0FBQyxJQUFJLEtBQUssY0FBSSxJQUFLLE9BQW1DLENBQUMsSUFBSSxLQUFLLDJCQUFpQixJQUFLLE9BQW1DLENBQUMsSUFBSSxLQUFLLGlCQUFPO2dDQUNqTCxPQUFtQyxDQUFDLElBQUksS0FBSywwQkFBZ0IsSUFBSyxPQUFtQyxDQUFDLElBQUksS0FBSyx3QkFBYyxJQUFLLE9BQW1DLENBQUMsSUFBSSxLQUFLLHdCQUFjO21DQUMxTCxPQUFtQyxDQUFDLElBQUksS0FBSyxnQkFBTSxJQUFLLE9BQW1DLENBQUMsSUFBSSxLQUFLLDRCQUFrQixJQUFLLE9BQW1DLENBQUMsSUFBSSxLQUFLLHNCQUFZO2dDQUN2TCxPQUFtQyxDQUFDLElBQUksS0FBSyxjQUFJLElBQUssT0FBbUMsQ0FBQyxJQUFJLEtBQUssc0JBQVk7Z0NBQzdHLE9BQU8sT0FBTyxDQUFDOzRCQUNuQixPQUFPLElBQUksQ0FBQzt3QkFDaEIsQ0FBQyxDQUFDO3dCQUNQLENBQUMsVUFBVSxJQUFJLENBQUMsS0FBSyxDQUFDLFNBQVMsS0FBSyxTQUFTLElBQUksQ0FBQyxLQUFLLENBQUMsU0FBUyxLQUFLLE1BQU0sSUFBSSxLQUFLLENBQUMsU0FBUyxLQUFLLEtBQUssQ0FBQyxDQUFDLENBQUMsQ0FBQzs0QkFDekcsOEJBQU0sTUFBTSxFQUFDLE9BQU8sRUFBQyxLQUFLLEVBQUUsRUFBRSxXQUFXLEVBQUUsQ0FBQyxFQUFFLE9BQU8sRUFBRSxPQUFPLENBQUEsQ0FBQyxDQUFDLEdBQUcsQ0FBQSxDQUFDLENBQUMsR0FBRyxFQUFFLEVBQUUsQ0FBQyxFQUFFLENBQUMsS0FBSyxDQUFDLFNBQVMsS0FBSyxZQUFZLENBQUMsQ0FBQztvQ0FDaEgsWUFBSyxhQUFhLENBQUMsQ0FBQyxDQUFDLGNBQUksU0FBUyxnQkFBTSxTQUFTLEdBQUcsWUFBWSxDQUFFLENBQUMsQ0FBQztvQ0FDcEUsWUFBSyxVQUFVLGNBQUksYUFBYSxDQUFDLENBQUMsQ0FBQyxnQkFBTSxRQUFRLEdBQUcsV0FBVyxDQUFFLENBQUMsR0FDaEU7NEJBQ0osQ0FBQyxDQUFDLElBQUk7d0JBQ1QsQ0FBQyxLQUFLLENBQUMsSUFBSSxLQUFLLFNBQVMsSUFBSSxLQUFLLENBQUMsSUFBSSxDQUFDLElBQUksU0FBUyxDQUFDLFFBQVEsQ0FBQyxNQUFNLENBQUMsQ0FBQyxDQUFDOzRCQUNyRSw4QkFBTSxXQUFXLEVBQUUsR0FBRyxFQUFFLElBQUksRUFBRSxPQUFPLEVBQ3BDLENBQUMsRUFBRSxTQUFTLEtBQUssaUJBQWlCLENBQUMsQ0FBQyxDQUFDLElBQUksQ0FBQyxHQUFHLENBQUMsVUFBVSxDQUFDLENBQUMsQ0FBQyxFQUFFLGFBQWEsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxVQUFVLEVBQzNGLENBQUMsRUFBRSxTQUFTLEtBQUssZUFBZSxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUMsR0FBRyxDQUFDLFVBQVUsQ0FBQyxDQUFDLENBQUMsRUFBRSxhQUFhLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsU0FBUyxFQUN4RixLQUFLLEVBQUUsU0FBUyxLQUFLLGlCQUFpQixDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUMsR0FBRyxDQUFDLFVBQVUsQ0FBQyxDQUFDLENBQUMsR0FBRyxhQUFhLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxRQUFRLEdBQUcsVUFBVSxHQUFHLFdBQVcsQ0FBQyxFQUMzSCxNQUFNLEVBQUUsU0FBUyxLQUFLLGVBQWUsQ0FBQyxDQUFDLENBQUMsSUFBSSxDQUFDLEdBQUcsQ0FBQyxVQUFVLENBQUMsQ0FBQyxDQUFDLEdBQUcsYUFBYSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsU0FBUyxHQUFHLFNBQVMsR0FBRyxZQUFZLENBQUMsR0FBSTs0QkFDaEksQ0FBQyxDQUFDLElBQUksQ0FDVjtvQkFDSCxDQUFDLFVBQVUsSUFBSSxLQUFLLENBQUMsWUFBWSxLQUFLLE1BQU0sQ0FBQyxDQUFDLENBQUMsQ0FBQyx5Q0FBSyxDQUFDLENBQUM7d0JBQ3ZELG9CQUFDLDRCQUFrQixJQUFDLE9BQU8sRUFBRSxDQUFDLEtBQUssQ0FBQyxHQUFHLEtBQUssU0FBUyxJQUFJLEtBQUssQ0FBQyxHQUFHLENBQUMsRUFDbEUsUUFBUSxFQUFFLEtBQUssQ0FBQyxJQUFJLEtBQUssU0FBUyxJQUFJLEtBQUssQ0FBQyxJQUFJLEVBQ2hELFNBQVMsRUFBRSxDQUFDLENBQUMsS0FBSyxDQUFDLEdBQUcsS0FBSyxTQUFTLElBQUksS0FBSyxDQUFDLElBQUksS0FBSyxTQUFTLElBQUksQ0FBQyxLQUFLLENBQUMsSUFBSSxJQUFJLENBQUMsS0FBSyxDQUFDLEdBQUcsQ0FBQyxFQUM5RixVQUFVLEVBQUUsS0FBSyxDQUFDLFFBQVEsS0FBSyxTQUFTLElBQUksUUFBUSxDQUFDLE9BQU8sQ0FBQyxJQUFJLEdBQUcsQ0FBQyxFQUNyRSxZQUFZLEVBQUUsS0FBSyxDQUFDLGFBQWEsS0FBSyxTQUFTLEVBQy9DLFdBQVcsRUFBRSxLQUFLLENBQUMsU0FBUyxLQUFLLFNBQVMsRUFDMUMsZ0JBQWdCLEVBQUUsWUFBWSxFQUM5QixZQUFZLEVBQUUsWUFBWSxFQUMxQixRQUFRLEVBQUUsS0FBSyxDQUFDLFlBQVksRUFDNUIsZUFBZSxFQUFFLFNBQVMsR0FBQyxFQUFFLEVBQzdCLFFBQVEsRUFBRSxhQUFhLEVBQ3ZCLENBQUMsRUFBRSxDQUFDLEtBQUssQ0FBQyxZQUFZLEtBQUssTUFBTSxDQUFDLENBQUMsQ0FBQyxFQUFFLENBQUMsQ0FBQyxDQUFDLENBQUMsUUFBUSxHQUFHLEVBQUUsR0FBRyxVQUFVLEdBQUcsRUFBRSxDQUFDLENBQUMsRUFDM0UsQ0FBQyxFQUFFLEVBQUUsNkJBQTBCLE1BQU0sSUFDcEMsS0FBSyxDQUFDLFFBQVEsQ0FBQyxHQUFHLENBQUMsS0FBSyxDQUFDLFFBQVEsRUFBRSxVQUFDLE9BQU87NEJBQ2pDLElBQUksQ0FBQyxLQUFLLENBQUMsY0FBYyxDQUFDLE9BQU8sQ0FBQztnQ0FDOUIsT0FBTyxJQUFJLENBQUM7NEJBQ2hCLElBQUssT0FBbUMsQ0FBQyxJQUFJLEtBQUssZ0JBQU07Z0NBQ3BELE9BQU8sT0FBTyxDQUFDOzRCQUNuQixPQUFPLElBQUksQ0FBQzt3QkFDaEIsQ0FBQyxDQUFDLENBQ1ksQ0FFckIsQ0FDSjtZQUNQLEtBQUssQ0FBQyxNQUFNLEtBQU0sU0FBUyxJQUFJLEtBQUssQ0FBQyxNQUFNLEtBQUssUUFBUSxDQUFDLENBQUM7Z0JBQzNELG9CQUFDLGdCQUFNLElBQUMsUUFBUSxFQUFFLEtBQUssQ0FBQyxNQUFNLEVBQUUsTUFBTSxFQUFFLFlBQVksRUFBRSxLQUFLLEVBQUUsV0FBVyxFQUFFLFVBQVUsRUFBRSxRQUFRLEVBQUUsV0FBVyxFQUFFLFNBQVMsRUFDdEgsa0JBQWtCLEVBQUUsd0JBQXdCLEVBQUUsbUJBQW1CLEVBQUUseUJBQXlCLEVBQUUsWUFBWSxFQUFFLFVBQVUsR0FDcEg7Z0JBQ0YsQ0FBQyxDQUFDLElBQUksQ0FDRCxDQUNNLENBQ3BCLENBQUE7QUFFSCxDQUFDLENBQUE7QUFFRCxrQkFBZSxJQUFJLENBQUMifQ==

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/PointNode.js":
/*!*****************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/PointNode.js ***!
  \*****************************************************************/
/***/ (function(__unused_webpack_module, exports) {

"use strict";

// ******************************************************************************************************
//  PointNode.tsx - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  03/18/2021 - C Lackner
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.PointNode = void 0;
var MaxPoints = 20;
/**
 *
 * Node in a tree.
 */
var PointNode = /** @class */ (function () {
    function PointNode(data) {
        var _this = this;
        this.dim = data[0].length;
        // That minimum time stamp that fits in this bucket
        this.minT = data[0][0];
        // The maximum time stamp that might fit in this bucket
        this.maxT = data[data.length - 1][0];
        // Intializing other vars
        this.avgV = Array(this.dim - 1).fill(0);
        this.minV = Array(this.dim - 1).fill(0);
        this.maxV = Array(this.dim - 1).fill(0);
        this.children = null;
        this.points = null;
        if (data.length <= MaxPoints) {
            if (data.some(function (point) { return point.length != _this.dim; }))
                throw new TypeError("Jagged data passed to PointNode. All points should all be ".concat(this.dim, " dimensions."));
            this.points = data;
            var _loop_1 = function (index_1) {
                this_1.minV[index_1 - 1] = Math.min.apply(Math, __spreadArray([], __read(data.filter(function (pt) { return !isNaN(pt[index_1]); }).map(function (pt) { return pt[index_1]; })), false));
            };
            var this_1 = this;
            for (var index_1 = 1; index_1 < this.dim; index_1++) {
                _loop_1(index_1);
            }
            var _loop_2 = function (index_2) {
                this_2.maxV[index_2 - 1] = Math.max.apply(Math, __spreadArray([], __read(data.filter(function (pt) { return !isNaN(pt[index_2]); }).map(function (pt) { return pt[index_2]; })), false));
            };
            var this_2 = this;
            for (var index_2 = 1; index_2 < this.dim; index_2++) {
                _loop_2(index_2);
            }
            return;
        }
        var nLevel = Math.floor(Math.pow(data.length, 1 / MaxPoints));
        var blockSize = nLevel * MaxPoints;
        var index = 0;
        this.children = [];
        while (index < data.length) {
            this.children.push(new PointNode(data.slice(index, index + blockSize)));
            index = index + blockSize;
        }
        var _loop_3 = function (index_3) {
            this_3.minV[index_3] = Math.min.apply(Math, __spreadArray([], __read(this_3.children.map(function (node) { return node.minV[index_3]; })), false));
        };
        var this_3 = this;
        for (var index_3 = 0; index_3 < this.dim - 1; index_3++) {
            _loop_3(index_3);
        }
        var _loop_4 = function (index_4) {
            this_4.maxV[index_4] = Math.max.apply(Math, __spreadArray([], __read(this_4.children.map(function (node) { return node.maxV[index_4]; })), false));
        };
        var this_4 = this;
        for (var index_4 = 0; index_4 < this.dim - 1; index_4++) {
            _loop_4(index_4);
        }
    }
    PointNode.prototype.GetData = function (Tstart, Tend, IncludeEdges) {
        var _this = this;
        if (this.points != null && Tstart <= this.minT && Tend >= this.maxT)
            return this.points;
        if (this.points != null && IncludeEdges !== undefined && IncludeEdges)
            return this.points.filter(function (pt, i) {
                var _a, _b;
                return (pt[0] >= Tstart && pt[0] <= Tend) ||
                    i < (((_b = (_a = _this.points) === null || _a === void 0 ? void 0 : _a.length) !== null && _b !== void 0 ? _b : 0) - 1) && (_this.points != null ? _this.points[i + 1][0] : 0) >= Tstart ||
                    i > 0 && (_this.points != null ? _this.points[i - 1][0] : 0) <= Tend;
            });
        if (this.points != null)
            return this.points.filter(function (pt) { return pt[0] >= Tstart && pt[0] <= Tend; });
        var result = [];
        return result.concat.apply(result, __spreadArray([], __read(this.children.filter(function (node) {
            return (node.minT <= Tstart && node.maxT > Tstart) ||
                (node.maxT >= Tend && node.minT < Tend) ||
                (node.minT >= Tstart && node.maxT <= Tend);
        }).map(function (node) { return node.GetData(Tstart, Tend, IncludeEdges); })), false));
    };
    PointNode.prototype.GetFullData = function () {
        return this.GetData(this.minT, this.maxT);
    };
    PointNode.prototype.GetAllLimits = function (Tstart, Tend) {
        var result = Array(this.dim - 1);
        for (var index = 0; index < this.dim - 1; index++)
            result[index] = this.GetLimits(Tstart, Tend, index);
        return result;
    };
    // Note: Dimension indexing does not include time, I.E. in (x,y), y would be dimension 0;
    PointNode.prototype.GetLimits = function (Tstart, Tend, dimension) {
        var currentIndex = dimension !== null && dimension !== void 0 ? dimension : 0;
        var max = this.maxV[currentIndex];
        var min = this.minV[currentIndex];
        if (this.points == null && !(Tstart <= this.minT && Tend > this.maxT)) {
            // Array represents all limits of buckets
            var limits = this.children.filter(function (n) { return n.maxT > Tstart && n.minT < Tend; }).map(function (n) { return n.GetLimits(Tstart, Tend, currentIndex); });
            min = Math.min.apply(Math, __spreadArray([], __read(limits.map(function (pt) { return pt[0]; })), false));
            max = Math.max.apply(Math, __spreadArray([], __read(limits.map(function (pt) { return pt[1]; })), false));
        }
        if (this.points != null && !(Tstart <= this.minT && Tend > this.maxT)) {
            // Array represents all numbers within this bucket that fall in range
            var limits = this.points.filter(function (pt) { return pt[0] > Tstart && pt[0] < Tend; }).map(function (pt) { return pt[currentIndex + 1]; });
            min = Math.min.apply(Math, __spreadArray([], __read(limits), false));
            max = Math.max.apply(Math, __spreadArray([], __read(limits), false));
        }
        return [min, max];
    };
    /**
     * Retrieves a point from the PointNode tree
     * @param {number} tVal - The time value of the point to retrieve from the tree.
     */
    PointNode.prototype.GetPoint = function (tVal) {
        return this.PointBinarySearch(tVal, 1)[0];
    };
    /**
     * Retrieves a specified number of points from the PointNode tree, centered around a point
     * @param {number} tVal - The time value of the center point of the point retrieval.
     * @param {number} pointsRetrieved - The number of points to retrieve
     */
    PointNode.prototype.GetPoints = function (tVal, pointsRetrieved) {
        if (pointsRetrieved === void 0) { pointsRetrieved = 1; }
        return this.PointBinarySearch(tVal, pointsRetrieved);
    };
    PointNode.prototype.PointBinarySearch = function (tVal, pointsRetrieved, bucketLowerNeighbor, bucketUpperNeighbor) {
        if (pointsRetrieved === void 0) { pointsRetrieved = 1; }
        if (pointsRetrieved <= 0)
            throw new RangeError("Requested number of points must be positive value.");
        // round tVal back to whole integer 
        if (this.points !== null) {
            // if the tVal is less than the minimum value of the subsection, return the first point
            if (tVal < this.minT) {
                var spillOver = pointsRetrieved - this.points.length;
                var spillOverPoints = (spillOver > 0 && bucketUpperNeighbor !== undefined) ? bucketUpperNeighbor.PointBinarySearch(tVal, spillOver, this, undefined) : [];
                return this.points.slice(0, pointsRetrieved).concat(spillOverPoints);
            }
            // if the tVal is greater than the largest value of the subsection, return the last point
            if (tVal > this.maxT) {
                var spillOver = pointsRetrieved - this.points.length;
                var spillOverPoints = (spillOver > 0 && bucketLowerNeighbor !== undefined) ? bucketLowerNeighbor.PointBinarySearch(tVal, spillOver, undefined, this) : [];
                return spillOverPoints.concat(this.points.slice(-pointsRetrieved));
            }
            // Otherwise, perform binary search
            var upper = this.points.length - 1;
            var lower = 0;
            var Tlower = this.minT;
            var Tupper = this.maxT;
            while (Tupper !== tVal && Tlower !== tVal && upper !== lower && Tupper !== Tlower) {
                var center = Math.round((upper + lower) / 2);
                var Tcenter = this.points[center][0];
                if (center === upper || center === lower)
                    break;
                if (Tcenter <= tVal)
                    lower = center;
                if (Tcenter > tVal)
                    upper = center;
                Tupper = this.points[upper][0];
                Tlower = this.points[lower][0];
            }
            var upperPoints = Math.floor(pointsRetrieved / 2);
            var lowerPoints = upperPoints;
            // Adjustment for even number of points
            var sidingAdjust = pointsRetrieved % 2 === 0 ? 1 : 0;
            var centerIndex = void 0;
            if (Math.abs(tVal - Tlower) < Math.abs(tVal - Tupper)) {
                centerIndex = lower;
                lowerPoints -= sidingAdjust;
            }
            else {
                centerIndex = upper;
                upperPoints -= sidingAdjust;
            }
            // Note: If we have spillover and no neighbor on the spillover side, then we discard the idea of spillover, and just return as many as we can on that side
            var upperSpillOver = centerIndex + upperPoints + 1 - this.points.length;
            var upperNeighborPoints = (upperSpillOver > 0 && bucketUpperNeighbor !== undefined) ? bucketUpperNeighbor.PointBinarySearch(tVal, upperSpillOver, this, undefined) : [];
            var lowerSpillOver = lowerPoints - centerIndex;
            var lowerNeighborPoints = (lowerSpillOver > 0 && bucketLowerNeighbor !== undefined) ? bucketLowerNeighbor.PointBinarySearch(tVal, lowerSpillOver, undefined, this) : [];
            return lowerNeighborPoints.concat(this.points.slice(Math.max(centerIndex - lowerPoints, 0), Math.min(centerIndex + upperPoints + 1, this.points.length))).concat(upperNeighborPoints);
        }
        else if (this.children !== null) {
            var childIndex = -1;
            // if the subsection is null, and the tVal is less than the minimum value of the subsection, ??Start over again looking for the point in the first subsection??
            if (tVal < this.minT)
                childIndex = 0;
            else if (tVal > this.maxT)
                childIndex = this.children.length - 1;
            else
                childIndex = this.children.findIndex(function (n) { return n.maxT >= tVal; });
            if (childIndex === -1)
                throw new RangeError("Could not find child bucket with point that has a time value of ".concat(tVal));
            // Find neighbors
            var upperNeighbor = childIndex !== this.children.length - 1 ? this.children[childIndex + 1] : undefined;
            var lowerNeighbor = childIndex !== 0 ? this.children[childIndex - 1] : undefined;
            return this.children[childIndex].PointBinarySearch(tVal, pointsRetrieved, lowerNeighbor, upperNeighbor);
        }
        else
            throw new RangeError("Both children and points are null for PointNode, unabled to find point with time value of ".concat(tVal));
    };
    return PointNode;
}());
exports.PointNode = PointNode;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiUG9pbnROb2RlLmpzIiwic291cmNlUm9vdCI6IiIsInNvdXJjZXMiOlsiLi4vc3JjL1BvaW50Tm9kZS50c3giXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6IjtBQUFBLHlHQUF5RztBQUN6Ryx3QkFBd0I7QUFDeEIsRUFBRTtBQUNGLHFFQUFxRTtBQUNyRSxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4RyxzR0FBc0c7QUFDdEcsd0ZBQXdGO0FBQ3hGLEVBQUU7QUFDRiwwQ0FBMEM7QUFDMUMsRUFBRTtBQUNGLHdHQUF3RztBQUN4Ryx3R0FBd0c7QUFDeEcsNEVBQTRFO0FBQzVFLEVBQUU7QUFDRiw4QkFBOEI7QUFDOUIsd0dBQXdHO0FBQ3hHLDBCQUEwQjtBQUMxQixtREFBbUQ7QUFDbkQsRUFBRTtBQUNGLHlHQUF5Rzs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7OztBQUV6RyxJQUFNLFNBQVMsR0FBRyxFQUFFLENBQUM7QUFFckI7OztHQUdHO0FBQ0g7SUFZSSxtQkFBWSxJQUFxQjtRQUFqQyxpQkFnQ0M7UUEvQkcsSUFBSSxDQUFDLEdBQUcsR0FBRyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUMsTUFBTSxDQUFDO1FBQzFCLG1EQUFtRDtRQUNuRCxJQUFJLENBQUMsSUFBSSxHQUFHLElBQUksQ0FBQyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztRQUN2Qix1REFBdUQ7UUFDdkQsSUFBSSxDQUFDLElBQUksR0FBRyxJQUFJLENBQUMsSUFBSSxDQUFDLE1BQU0sR0FBRyxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUMsQ0FBQztRQUNyQyx5QkFBeUI7UUFDekIsSUFBSSxDQUFDLElBQUksR0FBRyxLQUFLLENBQUMsSUFBSSxDQUFDLEdBQUcsR0FBQyxDQUFDLENBQUMsQ0FBQyxJQUFJLENBQUMsQ0FBQyxDQUFDLENBQUM7UUFDdEMsSUFBSSxDQUFDLElBQUksR0FBRyxLQUFLLENBQUMsSUFBSSxDQUFDLEdBQUcsR0FBQyxDQUFDLENBQUMsQ0FB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LFVBQUEsQ0FBQyxJQUFJLE9BQUEsQ0FBQyxDQUFDLElBQUksR0FBRyxNQUFNLElBQUksQ0FBQyxDQUFDLElBQUksR0FBRyxJQUFJLEVBQWhDLENBQWdDLENBQUMsQ0FBQyxHQUFHLENBQUMsVUFBQSxDQUFDLElBQUksT0FBQSxDQUFDLENBQUMsU0FBUyxDQUFDLE1BQU0sRUFBQyxJQUFJLEVBQUMsWUFBWSxDQUFDLEVBQXJDLENBQXFDLENBQUMsQ0FBQztZQUM1SCxHQUFHLEdBQUcsSUFBSSxDQUFDLEdBQUcsT0FBUixJQUFJLDJCQUFRLE1BQU0sQ0FBQyxHQUFHLENBQUMsVUFBQSxFQUFFLElBQUksT0FBQSxFQUFFLENBQUMsQ0FBQyxDQUFDLEVBQUwsQ0FBSyxDQUFDLFVBQUMsQ0FBQztZQUMzQyxHQUFHLEdBQUcsSUFBSSxDQUFDLEdBQUcsT0FBUixJQUFJLDJCQUFRLE1BQU0sQ0FBQyxHQUFHLENBQUMsVUFBQSxFQUFFLElBQUksT0FBQSxFQUFFLENBQUMsQ0FBQyxDQUFDLEVBQUwsQ0FBSyxDQUFDLFVBQUMsQ0FBQztRQUM3QyxDQUFDO1FBQ0QsSUFBSSxJQUFJLENBQUMsTUFBTSxJQUFJLElBQUksSUFBSSxDQUFDLENBQUMsTUFBTSxJQUFJLElBQUksQ0FBQyxJQUFJLElBQUksSUFBSSxHQUFHLElBQUksQ0FBQyxJQUFJLENBQUMsRUFBRSxDQUFDO1lBQ3RFLHFFQUFxRTtZQUNyRSxJQUFNLE1BQU0sR0FBRyxJQUFJLENBQUMsTUFBTyxDQUFDLE1BQU0sQ0FBQyxVQUFBLEVBQUUsSUFBSSxPQUFBLEVBQUUsQ0FBQyxDQUFDLENBQUMsR0FBRyxNQUFNLElBQUksRUFBRSxDQUFDLENBQUMsQ0FBQyxHQUFHLElBQUksRUFBOUIsQ0FBOEIsQ0FBQyxDQUFDLEdBQUcsQ0FBQyxVQUFBLEVBQUUsSUFBSSxPQUFBLEVBQUUsQ0FBQyxZQUFZLEdBQUMsQ0FBQyxDQUFDLEVBQWxCLENBQWtCLENBQUMsQ0FBQztZQUN2RyxHQUFHLEdBQUcsSUFBSSxDQUFDLEdBQUcsT0FBUixJQUFJLDJCQUFRLE1BQU0s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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/SymbolicMarker.js":
/*!**********************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/SymbolicMarker.js ***!
  \**********************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

// ******************************************************************************************************
//  Vertica;Marker.tsx - Gbtc
//
//  Copyright © 2022, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  04/29/2022 - C Lackner
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
var SymbolicMarker = function (props) {
    var context = React.useContext(GraphContext_1.GraphContext);
    var _a = __read(React.useState({ x: props.xPos, y: props.yPos }), 2), position = _a[0], setPosition = _a[1];
    var _b = __read(React.useState(false), 2), isSelected = _b[0], setSelected = _b[1];
    var _c = __read(React.useState(""), 2), guid = _c[0], setGuid = _c[1];
    var isInBounds = React.useCallback(function (xArg, yArg) {
        var _a, _b, _c, _d;
        var xP = ((_b = (_a = props.usePixelPositioning) === null || _a === void 0 ? void 0 : _a.x) !== null && _b !== void 0 ? _b : false) ? context.XApplyPixelOffset(props.xPos) : context.XTransformation(props.xPos);
        var xT = context.XTransformation(xArg);
        var yP = ((_d = (_c = props.usePixelPositioning) === null || _c === void 0 ? void 0 : _c.y) !== null && _d !== void 0 ? _d : false) ? context.YApplyPixelOffset(props.yPos) : context.YTransformation(props.yPos, GraphContext_1.AxisMap.get(props.axis));
        var yT = context.YTransformation(yArg, GraphContext_1.AxisMap.get(props.axis));
        // Note: This is actually a rectangular box
        return (xT <= xP + props.radius && xT >= xP - props.radius && yT <= yP + props.radius && yT >= yP - props.radius);
    }, [props.axis, props.yPos, props.yPos, props.radius, GraphContext_1.AxisMap, props.usePixelPositioning, context.XTransformation, context.YTransformation, context.XApplyPixelOffset, context.YApplyPixelOffset]);
    var onClick = React.useCallback(function (xArg, yArg) {
        if (isInBounds(xArg, yArg))
            setSelected(true);
    }, [setSelected, isInBounds]);
    var onMove = React.useCallback(function (xArg, yArg) {
        if (props.onHover !== undefined && isInBounds(xArg, yArg))
            props.onHover();
    }, [props.onHover, isInBounds]);
    React.useEffect(function () {
        var id = context.RegisterSelect({
            onRelease: function () { return setSelected(false); },
            onPlotLeave: function () { return setSelected(false); },
            onClick: onClick,
            onMove: onMove,
            axis: props.axis,
            allowSnapping: false
        });
        setGuid(id);
        return function () { context.RemoveSelect(id); };
    }, []);
    React.useEffect(function () {
        if (guid === "")
            return;
        context.UpdateSelect(guid, {
            onRelease: function () { return setSelected(false); },
            onPlotLeave: function () { return setSelected(false); },
            onClick: onClick,
            onMove: onMove,
            axis: props.axis,
            allowSnapping: false
        });
    }, [onClick, onMove]);
    React.useEffect(function () {
        setPosition({ x: props.xPos, y: props.yPos });
    }, [props.xPos, props.yPos]);
    React.useEffect(function () {
        if (props.setPosition === undefined)
            return;
        if (!isSelected && (props.xPos !== position.x || props.yPos !== position.y))
            props.setPosition(position.x, position.y);
    }, [isSelected, position]);
    React.useEffect(function () {
        if (context.CurrentMode !== 'select')
            setSelected(false);
    }, [context.CurrentMode]);
    React.useEffect(function () {
        if (isSelected)
            setPosition({ x: context.XHoverSnap, y: context.YHoverSnap[GraphContext_1.AxisMap.get(props.axis)] });
    }, [context.XHoverSnap, context.YHoverSnap]);
    return (React.createElement(React.Fragment, null,
        React.createElement(SymbolicGraphic, { style: props.style, x: props.xPos, y: props.yPos, r: props.radius, a: GraphContext_1.AxisMap.get(props.axis), inPixels: props.usePixelPositioning }, props.children),
        props.setPosition !== undefined && (props.xPos !== position.x || props.yPos !== position.y) ?
            React.createElement(SymbolicGraphic, { style: props.style, x: position.x, y: position.y, r: props.radius, a: GraphContext_1.AxisMap.get(props.axis), inPixels: props.usePixelPositioning }, props.children)
            : null));
};
var SymbolicGraphic = function (props) {
    var _a, _b, _c, _d;
    var context = React.useContext(GraphContext_1.GraphContext);
    var xPixels = ((_b = (_a = props.inPixels) === null || _a === void 0 ? void 0 : _a.x) !== null && _b !== void 0 ? _b : false) ? context.XApplyPixelOffset(props.x) : context.XTransformation(props.x);
    var yPixels = ((_d = (_c = props.inPixels) === null || _c === void 0 ? void 0 : _c.y) !== null && _d !== void 0 ? _d : false) ? context.YApplyPixelOffset(props.y) : context.YTransformation(props.y, props.a);
    return (React.createElement("foreignObject", { style: props.style, x: xPixels - props.r, y: yPixels - props.r, width: 2 * props.r, height: 2 * props.r }, props.children));
};
exports["default"] = SymbolicMarker;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiU3ltYm9saWNNYXJrZXIuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi9zcmMvU3ltYm9saWNNYXJrZXIudHN4Il0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiI7QUFBQSx5R0FBeUc7QUFDekcsNkJBQTZCO0FBQzdCLEVBQUU7QUFDRixxRUFBcUU7QUFDckUsRUFBRTtBQUNGLHdHQUF3RztBQUN4Ryx3R0FBd0c7QUFDeEcsc0dBQXNHO0FBQ3RHLHdGQUF3RjtBQUN4RixFQUFFO0FBQ0YsMENBQTBDO0FBQzFDLEVBQUU7QUFDRix3R0FBd0c7QUFDeEcsd0dBQXdHO0FBQ3hHLDRFQUE0RTtBQUM1RSxFQUFFO0FBQ0YsOEJBQThCO0FBQzlCLHdHQUF3RztBQUN4RywwQkFBMEI7QUFDMUIsbURBQW1EO0FBQ25ELEVBQUU7QUFDRix5R0FBeUc7Ozs7Ozs7Ozs7Ozs7Ozs7OztBQUd6Ryw2QkFBK0I7QUFDL0IsK0NBQWdGO0FBYWhGLElBQU0sY0FBYyxHQUFvQyxVQUFDLEtBQUs7SUFDNUQsSUFBTSxPQUFPLEdBQUcsS0FBSyxDQUFDLFVBQVUsQ0FBQywyQkFBWSxDQUFDLENBQUM7SUFDekMsSUFBQSxLQUFBLE9BQTBCLEtBQUssQ0FBQyxRQUFRLENBQXlCLEVBQUMsQ0FBQyxFQUFFLEtBQUssQ0FBQyxJQUFJLEVBQUUsQ0FBQyxFQUFFLEtBQUssQ0FBQyxJQUFJLEVBQUMsQ0FBQyxJQUFBLEVBQS9GLFFBQVEsUUFBQSxFQUFFLFdBQVcsUUFBMEUsQ0FBQztJQUNqRyxJQUFBLEtBQUEsT0FBNEIsS0FBSyxDQUFDLFFBQVEsQ0FBVSxLQUFLLENBQUMsSUFBQSxFQUF6RCxVQUFVLFFBQUEsRUFBRSxXQUFXLFFBQWtDLENBQUM7SUFDM0QsSUFBQSxLQUFBLE9BQWtCLEtBQUssQ0FBQyxRQUFRLENBQVMsRUFBRSxDQUFDLElBQUEsRUFBM0MsSUFBSSxRQUFBLEVBQUUsT0FBTyxRQUE4QixDQUFDO0lBRW5ELElBQU0sVUFBVSxHQUFHLEtBQUssQ0FBQyxXQUFXLENB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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/TimeAxis.js":
/*!****************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/TimeAxis.js ***!
  \****************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";
/* provided dependency */ var console = __webpack_require__(/*! console-browserify */ "./node_modules/console-browserify/index.js");

// ******************************************************************************************************
//  TimeAxis.tsx - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  03/18/2021 - C Lackner
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
var moment = __webpack_require__(/*! moment */ "webpack/sharing/consume/default/moment/moment?be5a");
var helper_functions_1 = __webpack_require__(/*! @gpa-gemstone/helper-functions */ "./node_modules/@gpa-gemstone/helper-functions/lib/index.js");
var lodash_1 = __webpack_require__(/*! lodash */ "webpack/sharing/consume/default/lodash/lodash?552c");
var msPerSecond = 1000.00;
var msPerMinute = msPerSecond * 60.0;
var msPerHour = msPerMinute * 60.0;
var msPerDay = msPerHour * 24.0;
var msPerYear = msPerDay * 365;
function TimeAxis(props) {
    var _a, _b, _c, _d;
    /*
      Used on bottom of Plot.
    */
    var context = React.useContext(GraphContext_1.GraphContext);
    var _e = __read(React.useState([]), 2), tick = _e[0], setTick = _e[1];
    var _f = __read(React.useState(0), 2), hLabel = _f[0], setHlabel = _f[1];
    var _g = __read(React.useState(0), 2), hAxis = _g[0], setHAxis = _g[1];
    var _h = __read(React.useState('YYYY'), 2), tFormat = _h[0], setTformat = _h[1];
    var _j = __read(React.useState(''), 2), dFormat = _j[0], setDformat = _j[1];
    var _k = __read(React.useState('Time'), 2), title = _k[0], setTitle = _k[1];
    // Adjust space for X Axis labels
    React.useEffect(function () {
        setHlabel(title !== undefined ? (0, helper_functions_1.GetTextHeight)("Segoe UI", '1em', title) : 0);
    }, [title]);
    // Adjust unit label
    React.useEffect(function () {
        if (props.label === undefined)
            setTitle(undefined);
        var titleFormat = "";
        var unitLabel = "";
        switch (tFormat) {
            case ('SSS'):
                titleFormat = "MMM Do, YYYY HH:mm:ss";
                unitLabel = " (ms)";
                break;
            case ('ss.SS'):
                titleFormat = "MMM Do, YYYY HH:mm";
                unitLabel = " (sec.ms)";
                break;
            case ('ss'):
                titleFormat = "MMM Do, YYYY HH:mm";
                unitLabel = " (sec)";
                break;
            case ('mm:ss'):
                titleFormat = "MMM Do, YYYY HH";
                unitLabel = " (min:sec)";
                break;
            case ('mm'):
                titleFormat = "MMM Do, YYYY HH";
                unitLabel = " (min)";
                break;
            case ('HH:mm'):
                titleFormat = "MMM Do, YYYY";
                unitLabel = " (hour:min)";
                break;
            case ('HH'):
                titleFormat = "MMM Do, YYYY";
                unitLabel = " (hour)";
                break;
            case ('DD HH'):
                titleFormat = "MMM YYYY";
                unitLabel = " (day hour)";
                break;
            case ('MM/DD'):
                titleFormat = "YYYY";
                unitLabel = " (month/day)";
                break;
            case ('MM YY'):
                titleFormat = "";
                unitLabel = " (month year)";
                break;
            case ('YYYY'):
                titleFormat = "";
                unitLabel = " (year)";
                break;
            default:
                console.warn("Unrecognized format: ".concat(tFormat));
                break;
        }
        if (props.label === '') {
            var formatedTitle = titleFormat === "" ? "Time" : formatTS(tick[0], titleFormat);
            setTitle(formatedTitle + unitLabel);
        }
        else
            setTitle(props.label + unitLabel);
    }, [tFormat, props.label, tick]);
    // Adjust space for X Tick labels
    React.useEffect(function () {
        var dX = Math.max.apply(Math, __spreadArray([], __read(tick.map(function (t) { return (0, helper_functions_1.GetTextHeight)("Segoe UI", '1em', formatTS(t, tFormat)); })), false));
        dX = (isFinite(dX) ? dX : 0) + 12;
        setHAxis(dX);
    }, [tick, tFormat]);
    React.useEffect(function () {
        if (hAxis + hLabel !== props.heightAxis)
            props.setHeight(hAxis + hLabel);
    }, [hAxis, hLabel, props.heightAxis, props.setHeight]);
    React.useEffect(function () {
        var _a;
        var deltaT = context.XDomain[1] - context.XDomain[0];
        if (deltaT === 0)
            return;
        var format = 'YYYY';
        var dateFormat = '';
        if (deltaT < msPerYear * 15 && deltaT >= msPerYear) {
            format = 'MM YY';
            dateFormat = '';
        }
        if (deltaT < msPerYear && deltaT >= 30 * msPerDay) {
            format = 'MM/DD';
            dateFormat = 'YY';
        }
        if (deltaT < 30 * msPerDay && deltaT >= 2 * msPerDay) {
            format = 'DD HH';
            dateFormat = 'YY';
        }
        if (deltaT < 2 * msPerDay && deltaT >= 30 * msPerHour) {
            format = 'HH';
            dateFormat = 'MM/DD';
        }
        if (deltaT < 30 * msPerHour && deltaT >= msPerHour) {
            format = 'HH:mm';
            dateFormat = 'MM/DD';
        }
        if (deltaT < msPerHour && deltaT >= 30 * msPerMinute) {
            format = 'mm';
            dateFormat = 'MM/DD HH';
        }
        if (deltaT < 30 * msPerMinute && deltaT >= msPerMinute) {
            format = 'mm:ss';
            dateFormat = 'MM/DD HH';
        }
        if (deltaT < msPerMinute && deltaT >= 30 * msPerSecond) {
            format = 'ss';
            dateFormat = 'MM/DD HH:mm';
        }
        if (deltaT < 30 * msPerSecond && deltaT >= msPerSecond) {
            format = 'ss.SS';
            dateFormat = 'MM/DD HH:mm';
        }
        if (deltaT < msPerSecond) {
            format = 'SSS';
            dateFormat = 'MM/DD HH:mm:ss';
        }
        var Tstart = moment.utc(context.XDomain[0]);
        var Tend = moment.utc(context.XDomain[1]);
        var Tdiff = moment.duration(moment.utc(context.XDomain[1]).diff(moment.utc(context.XDomain[0])));
        var Ttick = (0, lodash_1.cloneDeep)(Tstart);
        var step = 10;
        var stepType = 'y';
        if (Tdiff.asYears() >= 70) {
            step = 10;
            stepType = 'y';
            setTopOfYear(Ttick);
            Ttick.year(Math.floor((Ttick.year()) / 10.0) * 10.0);
        }
        if (Tdiff.asYears() < 70 && Tdiff.asYears() >= 40) {
            step = 5;
            setTopOfYear(Ttick);
            Ttick.year(Math.floor((Ttick.year()) / 5.0) * 5.0);
        }
        if (Tdiff.asYears() < 40 && Tdiff.asYears() >= 15) {
            step = 2;
            setTopOfYear(Ttick);
            Ttick.year(Math.floor((Ttick.year()) / 2.0) * 2.0);
        }
        if (Tdiff.asYears() < 15 && Tdiff.asYears() >= 6) {
            stepType = 'M';
            step = 12;
            setTopOfYear(Ttick);
        }
        if (Tdiff.asYears() < 6 && Tdiff.asYears() >= 4) {
            stepType = 'M';
            step = 6;
            setTopOfMonth(Ttick);
            Ttick.month(Math.floor((Ttick.month()) / 6.0) * 6.0);
        }
        if (Tdiff.asYears() < 4 && Tdiff.asYears() >= 1.5) {
            stepType = 'M';
            step = 3;
            setTopOfMonth(Ttick);
            Ttick.month(Math.floor((Ttick.month()) / 3.0) * 3.0);
        }
        if (Tdiff.asYears() < 1.5 && Tdiff.asMonths() >= 6) {
            stepType = 'M';
            step = 1;
            setTopOfMonth(Ttick);
        }
        if (Tdiff.asMonths() < 6 && Tdiff.asMonths() >= 2) {
            stepType = 'w';
            step = 2;
            setTopOfWeek(Ttick);
        }
        if (Tdiff.asMonths() < 2 && Tdiff.asMonths() >= 1) {
            stepType = 'w';
            step = 1;
            setTopOfWeek(Ttick);
        }
        if (Tdiff.asMonths() < 1 && Tdiff.asDays() >= 16) {
            stepType = 'd';
            step = 2;
            setTopOfDay(Ttick);
        }
        if (Tdiff.asDays() < 16 && Tdiff.asDays() >= 10) {
            stepType = 'd';
            step = 1;
            setTopOfDay(Ttick);
        }
        if (Tdiff.asDays() < 10 && Tdiff.asDays() >= 3) {
            stepType = 'h';
            step = 12;
            setTopOfHour(Ttick);
            Ttick.hours(Math.floor((Ttick.hours()) / 12.0) * 12.0);
        }
        if (Tdiff.asDays() < 3 && Tdiff.asHours() >= 30) {
            stepType = 'h';
            step = 6;
            setTopOfHour(Ttick);
            Ttick.hours(Math.floor((Ttick.hours()) / 6.0) * 6.0);
        }
        if (Tdiff.asHours() < 30 && Tdiff.asHours() >= 18) {
            stepType = 'h';
            step = 3;
            setTopOfHour(Ttick);
            Ttick.hours(Math.floor((Ttick.hours()) / 3.0) * 3.0);
        }
        if (Tdiff.asHours() < 18 && Tdiff.asHours() >= 6) {
            stepType = 'h';
            step = 1;
            setTopOfHour(Ttick);
        }
        if (Tdiff.asHours() < 6 && Tdiff.asHours() >= 3) {
            stepType = 'm';
            step = 30;
            setTopOfMinute(Ttick);
            Ttick.minutes(Math.floor((Ttick.minutes()) / 30.0) * 30.0);
        }
        if (Tdiff.asHours() < 3 && Tdiff.asHours() >= 1) {
            stepType = 'm';
            step = 15;
            setTopOfMinute(Ttick);
            Ttick.minutes(Math.floor((Ttick.minutes()) / 15.0) * 15.0);
        }
        if (Tdiff.asHours() < 1 && Tdiff.asMinutes() >= 20) {
            stepType = 'm';
            step = 5;
            setTopOfMinute(Ttick);
            Ttick.minutes(Math.floor((Ttick.minutes()) / 5.0) * 5.0);
        }
        if (Tdiff.asMinutes() < 20 && Tdiff.asMinutes() >= 10) {
            stepType = 'm';
            step = 2;
            setTopOfMinute(Ttick);
            Ttick.minutes(Math.floor((Ttick.minutes()) / 2.0) * 2.0);
        }
        if (Tdiff.asMinutes() < 10 && Tdiff.asMinutes() >= 5) {
            stepType = 'm';
            step = 1;
            setTopOfMinute(Ttick);
        }
        if (Tdiff.asMinutes() < 5 && Tdiff.asMinutes() >= 2) {
            stepType = 's';
            step = 30;
            setTopOfSecond(Ttick);
            Ttick.second(Math.floor((Ttick.second()) / 30) * 30.0);
        }
        if (Tdiff.asMinutes() < 2 && Tdiff.asMinutes() >= 1) {
            stepType = 's';
            step = 15;
            setTopOfSecond(Ttick);
            Ttick.second(Math.floor((Ttick.second()) / 15) * 15.0);
        }
        if (Tdiff.asMinutes() < 1 && Tdiff.asSeconds() >= 30) {
            stepType = 's';
            step = 5;
            setTopOfSecond(Ttick);
            Ttick.second(Math.floor((Ttick.second()) / 5) * 5.0);
        }
        if (Tdiff.asSeconds() < 30 && Tdiff.asSeconds() >= 15) {
            stepType = 's';
            step = 2;
            setTopOfSecond(Ttick);
        }
        if (Tdiff.asSeconds() < 15 && Tdiff.asSeconds() >= 5) {
            stepType = 's';
            step = 1;
            setTopOfSecond(Ttick);
        }
        if (Tdiff.asSeconds() < 5 && Tdiff.asSeconds() >= 2) {
            stepType = 'ms';
            step = 500;
            setTopOfms(Ttick);
            Ttick.millisecond(Math.floor((Ttick.millisecond()) / 500) * 500.0);
        }
        if (Tdiff.asSeconds() < 2 && Tdiff.asSeconds() >= 1) {
            stepType = 'ms';
            step = 250;
            setTopOfms(Ttick);
            Ttick.millisecond(Math.floor((Ttick.millisecond()) / 250) * 250.0);
        }
        if (Tdiff.asSeconds() < 1 && Tdiff.asMilliseconds() >= 500) {
            stepType = 'ms';
            step = 100;
            setTopOfms(Ttick);
            Ttick.millisecond(Math.floor((Ttick.millisecond()) / 100) * 100.0);
        }
        if (Tdiff.asMilliseconds() < 500 && Tdiff.asMilliseconds() >= 100) {
            stepType = 'ms';
            step = 50;
            setTopOfms(Ttick);
            Ttick.millisecond(Math.floor((Ttick.millisecond()) / 50) * 50.0);
        }
        if (Tdiff.asMilliseconds() < 100 && Tdiff.asMilliseconds() >= 20) {
            stepType = 'ms';
            step = 10;
            setTopOfms(Ttick);
            Ttick.millisecond(Math.floor((Ttick.millisecond()) / 10) * 10.0);
        }
        if (Tdiff.asMilliseconds() < 20) {
            stepType = 'ms';
            setTopOfms(Ttick);
            step = 1;
        }
        var newTicks = [Ttick.add(step, stepType)];
        while (newTicks[newTicks.length - 1] < Tend)
            newTicks.push(newTicks[newTicks.length - 1].clone().add(step, stepType));
        newTicks.pop();
        setTick(newTicks.map(function (t) { return t.valueOf(); }));
        setTformat(format);
        if ((_a = props.showDate) !== null && _a !== void 0 ? _a : false)
            setDformat(dateFormat);
        else
            setDformat('');
    }, [context.XDomain, props.showDate]);
    function setTopOfms(d) {
        d.milliseconds(Math.floor(d.millisecond()));
    }
    function setTopOfSecond(d) {
        setTopOfms(d);
        d.milliseconds(0);
    }
    function setTopOfMinute(d) {
        setTopOfSecond(d);
        d.seconds(0);
    }
    function setTopOfHour(d) {
        setTopOfMinute(d);
        d.minutes(0);
    }
    function setTopOfDay(d) {
        setTopOfHour(d);
        d.hours(0);
    }
    function setTopOfWeek(d) {
        setTopOfDay(d);
        d.weekday(0);
    }
    function setTopOfMonth(d) {
        setTopOfDay(d);
        d.date(1);
    }
    function setTopOfYear(d) {
        setTopOfDay(d);
        d.dayOfYear(0);
    }
    function formatTS(t, f) {
        var TS = moment.utc(t);
        return TS.format(f);
    }
    return (React.createElement("g", null,
        React.createElement("path", { stroke: 'black', style: { strokeWidth: 1 }, d: "M ".concat(props.offsetLeft - (((_a = props.showLeftMostTick) !== null && _a !== void 0 ? _a : true) ? 0 : 8), " ").concat(props.height - props.offsetBottom, " H ").concat(props.width - props.offsetRight + (((_b = props.showRightMostTick) !== null && _b !== void 0 ? _b : true) ? 0 : 8)) }),
        ((_c = props.showLeftMostTick) !== null && _c !== void 0 ? _c : true) ? React.createElement("path", { stroke: 'black', style: { strokeWidth: 1 }, d: "M ".concat(props.offsetLeft, " ").concat(props.height - props.offsetBottom, " v ").concat(8) }) : null,
        ((_d = props.showRightMostTick) !== null && _d !== void 0 ? _d : true) ? React.createElement("path", { stroke: 'black', style: { strokeWidth: 1 }, d: "M ".concat(props.width - props.offsetRight, " ").concat(props.height - props.offsetBottom, " v ").concat(8) }) : null,
        props.showTicks === undefined || props.showTicks ?
            React.createElement(React.Fragment, null,
                tick.map(function (l, i) { return React.createElement("path", { key: i, stroke: 'black', style: { strokeWidth: 1, transition: 'd 0.5s' }, d: "M ".concat(context.XTransformation(l), " ").concat(props.height - props.offsetBottom + 6, " v ").concat(-6) }); }),
                tick.map(function (l, i) { return React.createElement("text", { fill: 'black', key: i, style: { fontSize: '1em', textAnchor: 'middle', dominantBaseline: 'hanging', transition: 'x 0.5s, y 0.5s' }, y: props.height - props.offsetBottom + 8, x: context.XTransformation(l) }, formatTS(l, tFormat)); }))
            : null,
        title !== undefined ? React.createElement("text", { fill: 'black', style: { fontSize: '1em', textAnchor: 'middle', dominantBaseline: 'middle' }, x: props.offsetLeft + ((props.width - props.offsetLeft - props.offsetRight) / 2), y: props.height - props.offsetBottom + hAxis }, title) : null,
        (dFormat !== '' && tick.length > 0) ? React.createElement("text", { fill: 'black', style: { fontSize: '1em', textAnchor: 'end', dominantBaseline: 'middle' }, x: props.width - props.offsetRight, y: props.height - props.offsetBottom + hAxis }, formatTS(tick[0], dFormat)) : null));
}
exports["default"] = React.memo(TimeAxis);
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiVGltZUF4aXMuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi9zcmMvVGltZUF4aXMudHN4Il0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiI7QUFBQSx5R0FBeUc7QUFDekcsdUJBQXVCO0FBQ3ZCLEVBQUU7QUFDRixxRUFBcUU7QUFDckUsRUFBRTtBQUNGLHdHQUF3RztBQUN4Ryx3R0FBd0c7QUFDeEcsc0dBQXNHO0FBQ3RHLHdGQUF3RjtBQUN4RixFQUFFO0FBQ0YsMENBQTBDO0FBQzFDLEVBQUU7QUFDRix3R0FBd0c7QUFDeEcsd0dBQXdHO0FBQ3hHLDRFQUE0RTtBQUM1RSxFQUFFO0FBQ0YsOEJBQThCO0FBQzlCLHdHQUF3RztBQUN4RywwQkFBMEI7QUFDMUIsbURBQW1EO0FBQ25ELEVBQUU7QUFDRix5R0FBeUc7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7OztBQUV6Ryw2QkFBK0I7QUFDL0IsK0NBQTJDO0FBQzNDLCtCQUFpQztBQUNqQyxtRUFBNkQ7QUFDN0QsaUNBQW1DO0FBaUJuQyxJQUFNLFdBQVcsR0FBRyxPQUFPLENBQUM7QUFDNUIsSUFBTSxXQUFXLEdBQUcsV0FBVyxHQUFFLElBQUksQ0FBQztBQUN0QyxJQUFNLFNBQVMsR0FBRyxXQUFXLEdBQUUsSUFBSSxDQUFDO0FBQ3BDLElBQU0sUUFBUSxHQUFHLFNBQVMsR0FBRSxJQUFJLENBQUM7QUFDakMsSUFBTSxTQUFTLEdBQUcsUUFBUSxHQUFFLEdBQUcsQ0FBQztBQUtoQyxTQUFTLFFBQVEsQ0FBQyxLQUFhOztJQUMzQjs7TUFFRTtJQUNGLElBQU0sT0FBTyxHQUFHLEtBQUssQ0FBQyxVQUFVLENBQUMsMkJBQVksQ0FBQyxDQUFBO0lBQ3hDLElBQUEsS0FBQSxPQUFpQixLQUFLLENBQUMsUUFBUSxDQUFXLEVBQUUsQ0FBQyxJQUFBLEVBQTVDLElBQUksUUFBQSxFQUFDLE9BQU8sUUFBZ0MsQ0FBQztJQUM5QyxJQUFBLEtBQUEsT0FBc0IsS0FBSyxDQUFDLFFBQVEsQ0FBUyxDQUFDLENB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Q2xELElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxZQUFZLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDcEIsS0FBSyxDQUFDLElBQUksQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsS0FBSyxDQUFDLElBQUksRUFBRSxDQUFDLEdBQUcsR0FBRyxDQUFDLEdBQUcsR0FBRyxDQUFDLENBQUM7UUFDckQsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLE9BQU8sRUFBRSxHQUFHLEVBQUUsSUFBSSxLQUFLLENBQUMsT0FBTyxFQUFFLElBQUksQ0FBQyxFQUFFLENBQUM7WUFDakQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxFQUFFLENBQUM7WUFDVixZQUFZLENBQUMsS0FBSyxDQUFDLENBQUM7UUFDdEIsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLE9BQU8sRUFBRSxHQUFHLENBQUMsSUFBSSxLQUFLLENBQUMsT0FBTyxFQUFFLElBQUksQ0FBQyxFQUFFLENBQUM7WUFDaEQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxhQUFhLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDckIsS0FBSyxDQUFDLEtBQUssQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsS0FBSyxDQUFDLEtBQUssRUFBRSxDQUFDLEdBQUcsR0FBRyxDQUFDLEdBQUcsR0FBRyxDQUFDLENBQUM7UUFDdkQsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLE9BQU8sRUFBRSxHQUFHLENBQUMsSUFBSSxLQUFLLENBQUMsT0FBTyxFQUFFLElBQUksR0FBRyxFQUFFLENBQUM7WUFDbEQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxhQUFhLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDckIsS0FBSyxDQUFDLEtBQUssQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsS0FBSyxDQUFDLEtBQUssRUFBRSxDQUFDLEdBQUcsR0FBRyxDQUFDLEdBQUcsR0FBRyxDQUFDLENBQUM7UUFDdkQsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLE9BQU8sRUFBRSxHQUFHLEdBQUcsSUFBSSxLQUFLLENBQUMsUUFBUSxFQUFFLElBQUksQ0FBQyxFQUFFLENBQUM7WUFDbkQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxhQUFhLENBQUMsS0FBSyxDQUFDLENBQUM7UUFDdkIsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLFFBQVEsRUFBRSxHQUFHLENBQUMsSUFBSSxLQUFLLENBQUMsUUFBUSxFQUFFLElBQUksQ0FBQyxFQUFFLENBQUM7WUFDbEQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxZQUFZLENBQUMsS0FBSyxDQUFDLENBQUM7UUFDdEIsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLFFBQVEsRUFBRSxHQUFHLENBQUMsSUFBSSxLQUFLLENBQUMsUUFBUSxFQUFFLElBQUksQ0FBQyxFQUFFLENBQUM7WUFDbEQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxZQUFZLENBQUMsS0FBSyxDQUFDLENBQUM7UUFDdEIsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLFFBQVEsRUFBRSxHQUFHLENBQUMsSUFBSSxLQUFLLENBQUMsTUFBTSxFQUFFLElBQUksRUFBRSxFQUFFLENBQUM7WUFDakQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxXQUFXLENBQUMsS0FBSyxDQUFDLENBQUM7UUFDckIsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLE1BQU0sRUFBRSxHQUFHLEVBQUUsSUFBSSxLQUFLLENBQUMsTUFBTSxFQUFFLElBQUksRUFBRSxFQUFFLENBQUM7WUFDaEQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxXQUFXLENBQUMsS0FBSyxDQUFDLENBQUM7UUFDckIsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLE1BQU0sRUFBRSxHQUFHLEVBQUUsSUFBSSxLQUFLLENBQUMsTUFBTSxFQUFFLElBQUksQ0FBQyxFQUFFLENBQUM7WUFDL0MsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxFQUFFLENBQUM7WUFDVixZQUFZLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDcEIsS0FBSyxDQUFDLEtBQUssQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsS0FBSyxDQUFDLEtBQUssRUFBRSxDQUFDLEdBQUcsSUFBSSxDQUFDLEdBQUcsSUFBSSxDQUFDLENBQUM7UUFFekQsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLE1BQU0sRUFBRSxHQUFHLENBQUMsSUFBSSxLQUFLLENBQUMsT0FBTyxFQUFFLElBQUksRUFBRSxFQUFFLENBQUM7WUFDaEQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxZQUFZLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDcEIsS0FBSyxDQUFDLEtBQUssQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsS0FBSyxDQUFDLEtBQUssRUFBRSxDQUFDLEdBQUcsR0FBRyxDQUFDLEdBQUcsR0FBRyxDQUFDLENBQUE7UUFDdEQsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLE9BQU8sRUFBRSxHQUFHLEVBQUUsSUFBSSxLQUFLLENBQUMsT0FBTyxFQUFFLElBQUksRUFBRSxFQUFFLENBQUM7WUFDbEQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxZQUFZLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDcEIsS0FBSyxDQUFDLEtBQUssQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsS0FBSyxDQUFDLEtBQUssRUFBRSxDQUFDLEdBQUcsR0FBRyxDQUFDLEdBQUcsR0FBRyxDQUFDLENBQUE7UUFDdEQsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLE9BQU8sRUFBRSxHQUFHLEVBQUUsSUFBSSxLQUFLLENBQUMsT0FBTyxFQUFFLElBQUksQ0FBQyxFQUFFLENBQUM7WUFDakQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxZQUFZLENBQUMsS0FBSyxDQUFDLENBQUM7UUFDdEIsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLE9BQU8sRUFBRSxHQUFHLENBQUMsSUFBSSxLQUFLLENBQUMsT0FBTyxFQUFFLElBQUksQ0FBQyxFQUFFLENBQUM7WUFDaEQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxFQUFFLENBQUM7WUFDVixjQUFjLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDdEIsS0FBSyxDQUFDLE9BQU8sQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsS0FBSyxDQUFDLE9BQU8sRUFBRSxDQUFDLEdBQUcsSUFBSSxDQUFDLEdBQUcsSUFBSSxDQUFDLENBQUE7UUFDNUQsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLE9BQU8sRUFBRSxHQUFHLENBQUMsSUFBSSxLQUFLLENBQUMsT0FBTyxFQUFFLElBQUksQ0FBQyxFQUFFLENBQUM7WUFDaEQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxFQUFFLENBQUM7WUFDVixjQUFjLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDdEIsS0FBSyxDQUFDLE9BQU8sQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsS0FBSyxDQUFDLE9BQU8sRUFBRSxDQUFDLEdBQUcsSUFBSSxDQUFDLEdBQUcsSUFBSSxDQUFDLENBQUE7UUFDNUQsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLE9BQU8sRUFBRSxHQUFHLENBQUMsSUFBSSxLQUFLLENBQUMsU0FBUyxFQUFFLElBQUksRUFBRSxFQUFFLENBQUM7WUFDbkQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxjQUFjLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDdEIsS0FBSyxDQUFDLE9BQU8sQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsS0FBSyxDQUFDLE9BQU8sRUFBRSxDQUFDLEdBQUcsR0FBRyxDQUFDLEdBQUcsR0FBRyxDQUFDLENBQUE7UUFDMUQsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLFNBQVMsRUFBRSxHQUFHLEVBQUUsSUFBSSxLQUFLLENBQUMsU0FBUyxFQUFFLElBQUksRUFBRSxFQUFFLENBQUM7WUFDdEQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxjQUFjLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDdEIsS0FBSyxDQUFDLE9BQU8sQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsS0FBSyxDQUFDLE9BQU8sRUFBRSxDQUFDLEdBQUcsR0FBRyxDQUFDLEdBQUcsR0FBRyxDQUFDLENBQUE7UUFDMUQsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLFNBQVMsRUFBRSxHQUFHLEVBQUUsSUFBSSxLQUFLLENBQUMsU0FBUyxFQUFFLElBQUksQ0FBQyxFQUFFLENBQUM7WUFDckQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxjQUFjLENBQUMsS0FBSyxDQUFDLENBQUM7UUFDeEIsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLFNBQVMsRUFBRSxHQUFHLENBQUMsSUFBSSxLQUFLLENBQUMsU0FBUyxFQUFFLElBQUksQ0FBQyxFQUFFLENBQUM7WUFDcEQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxFQUFFLENBQUM7WUFDVixjQUFjLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDdEIsS0FBSyxDQUFDLE1BQU0sQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsS0FBSyxDQUFDLE1BQU0sRUFBRSxDQUFDLEdBQUcsRUFBRSxDQUFDLEdBQUcsSUFBSSxDQUFDLENBQUE7UUFDeEQsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLFNBQVMsRUFBRSxHQUFHLENBQUMsSUFBSSxLQUFLLENBQUMsU0FBUyxFQUFFLElBQUksQ0FBQyxFQUFFLENBQUM7WUFDcEQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxFQUFFLENBQUM7WUFDVixjQUFjLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDdEIsS0FBSyxDQUFDLE1BQU0sQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsS0FBSyxDQUFDLE1BQU0sRUFBRSxDQUFDLEdBQUcsRUFBRSxDQUFDLEdBQUcsSUFBSSxDQUFDLENBQUE7UUFDeEQsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLFNBQVMsRUFBRSxHQUFHLENBQUMsSUFBSSxLQUFLLENBQUMsU0FBUyxFQUFFLElBQUksRUFBRSxFQUFFLENBQUM7WUFDckQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxjQUFjLENBQUMsS0FBSyxDQUFDLENBQUM7WUFDdEIsS0FBSyxDQUFDLE1BQU0sQ0FBQyxJQUFJLENBQUMsS0FBSyxDQUFDLENBQUMsS0FBSyxDQUFDLE1BQU0sRUFBRSxDQUFDLEdBQUcsQ0FBQyxDQUFDLEdBQUcsR0FBRyxDQUFDLENBQUE7UUFDdEQsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLFNBQVMsRUFBRSxHQUFHLEVBQUUsSUFBSSxLQUFLLENBQUMsU0FBUyxFQUFFLElBQUksRUFBRSxFQUFFLENBQUM7WUFDdEQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxjQUFjLENBQUMsS0FBSyxDQUFDLENBQUM7UUFDeEIsQ0FBQztRQUNELElBQUksS0FBSyxDQUFDLFNBQVMsRUFBRSxHQUFHLEVBQUUsSUFBSSxLQUFLLENBQUMsU0FBUyxFQUFFLElBQUksQ0FBQyxFQUFFLENBQUM7WUFDckQsUUFBUSxHQUFHLEdBQUcsQ0FBQztZQUNmLElBQUksR0FBRyxDQUFDLENBQUM7WUFDVCxjQUFjLENBQUMsS0FBSyxDQUFDLENBQUM7UUFDeEIsQ0FB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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/VerticalMarker.js":
/*!**********************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/VerticalMarker.js ***!
  \**********************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

// ******************************************************************************************************
//  Vertica;Marker.tsx - Gbtc
//
//  Copyright © 2022, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  04/29/2022 - C Lackner
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
function VerticalMarker(props) {
    /*
      Marks an X Value vertically as a line.
    */
    var context = React.useContext(GraphContext_1.GraphContext);
    var _a = __read(React.useState(props.Value), 2), value = _a[0], setValue = _a[1];
    var _b = __read(React.useState(false), 2), isSelected = _b[0], setSelected = _b[1];
    var _c = __read(React.useState(""), 2), guid = _c[0], setGuid = _c[1];
    function generateData(v) {
        var axis = GraphContext_1.AxisMap.get(props.axis);
        var y1 = (props.start === undefined ? context.YDomain[axis][0] : props.start);
        var y2 = (props.end === undefined ? context.YDomain[axis][1] : props.end);
        return "M ".concat(context.XTransformation(v), " ").concat(context.YTransformation(y1, axis), " L ").concat(context.XTransformation(v), " ").concat(context.YTransformation(y2, axis));
    }
    var onClick = React.useCallback(function (x, _) {
        var xP = context.XTransformation(props.Value);
        var xT = context.XTransformation(x);
        if (xT <= xP + (props.width / 2) && xT >= xP - (props.width / 2))
            setSelected(true);
    }, [props.width, props.Value, context.XTransformation]);
    React.useEffect(function () {
        var id = context.RegisterSelect({
            onClick: onClick,
            onRelease: function (_) { return setSelected(false); },
            onPlotLeave: function (_) { return setSelected(false); },
            axis: props.axis,
            allowSnapping: false
        });
        setGuid(id);
        return function () { context.RemoveSelect(id); };
    }, []);
    React.useEffect(function () {
        if (guid === "")
            return;
        context.UpdateSelect(guid, {
            onClick: onClick,
            onRelease: function (_) { return setSelected(false); },
            onPlotLeave: function (_) { return setSelected(false); },
            axis: props.axis,
            allowSnapping: false
        });
    }, [onClick]);
    React.useEffect(function () {
        setValue(props.Value);
    }, [props.Value]);
    React.useEffect(function () {
        if (isSelected && props.setValue !== undefined && props.Value !== value)
            props.setValue(value);
    }, [isSelected, value]);
    React.useEffect(function () {
        if (isSelected && props.onClick !== undefined)
            props.onClick(props.Value);
    }, [isSelected]);
    React.useEffect(function () {
        if (context.CurrentMode !== 'select')
            setSelected(false);
    }, [context.CurrentMode]);
    React.useEffect(function () {
        if (isSelected)
            setValue(context.XHoverSnap);
    }, [context.XHoverSnap]);
    return (React.createElement("g", null,
        React.createElement("path", { d: generateData(props.Value), style: { fill: 'none', strokeWidth: props.width, stroke: props.color }, strokeDasharray: GraphContext_1.LineMap.get(props.lineStyle) }),
        props.setValue !== undefined && props.Value !== value && isSelected ?
            React.createElement("path", { d: generateData(value), style: { fill: 'none', strokeWidth: props.width, stroke: props.color, opacity: 0.5 }, strokeDasharray: GraphContext_1.LineMap.get(props.lineStyle) })
            : null));
}
exports["default"] = VerticalMarker;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiVmVydGljYWxNYXJrZXIuanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi9zcmMvVmVydGljYWxNYXJrZXIudHN4Il0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiI7QUFBQSx5R0FBeUc7QUFDekcsNkJBQTZCO0FBQzdCLEVBQUU7QUFDRixxRUFBcUU7QUFDckUsRUFBRTtBQUNGLHdHQUF3RztBQUN4Ryx3R0FBd0c7QUFDeEcsc0dBQXNHO0FBQ3RHLHdGQUF3RjtBQUN4RixFQUFFO0FBQ0YsMENBQTBDO0FBQzFDLEVBQUU7QUFDRix3R0FBd0c7QUFDeEcsd0dBQXdHO0FBQ3hHLDRFQUE0RTtBQUM1RSxFQUFFO0FBQ0YsOEJBQThCO0FBQzlCLHdHQUF3RztBQUN4RywwQkFBMEI7QUFDMUIsbURBQW1EO0FBQ25ELEVBQUU7QUFDRix5R0FBeUc7Ozs7Ozs7Ozs7Ozs7Ozs7OztBQUd6Ryw2QkFBK0I7QUFDL0IsK0NBQW9HO0FBY3BHLFNBQVMsY0FBYyxDQUFDLEtBQWE7SUFDbkM7O01BRUU7SUFDRixJQUFNLE9BQU8sR0FBRyxLQUFLLENBQUMsVUFBVSxDQUFDLDJCQUFZLENBQUMsQ0FBQTtJQUN4QyxJQUFBLEtBQUEsT0FBb0IsS0FBSyxDQUFDLFFBQVEsQ0FBUyxLQUFLLENBQUMsS0FBSyxDQUFDLElBQUEsRUFBdEQsS0FBSyxRQUFBLEVBQUUsUUFBUSxRQUF1QyxDQUFDO0lBQ3hELElBQUEsS0FBQSxPQUE0QixLQUFLLENBQUMsUUFBUSxDQUFVLEtBQUssQ0FBQyxJQUFBLEVBQXpELFVBQVUsUUFBQSxFQUFFLFdBQVcsUUFBa0MsQ0FBQztJQUMzRCxJQUFBLEtBQUEsT0FBa0IsS0FBSyxDQUFDLFFBQVEsQ0FBUyxFQUFFLENBQUMsSUFBQSxFQUEzQyxJQUFJLFFBQUEsRUFBRSxPQUFPLFFBQThCLENBQUM7SUFFbkQsU0FBUyxZQUFZLENBQUMsQ0FBUztRQUMzQixJQUFNLElBQUksR0FBRyxzQkFBTyxDQUFDLEdBQUcsQ0FBQyxLQUFLLENBQUMsSUFBSSxD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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/XValueAxis.js":
/*!******************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/XValueAxis.js ***!
  \******************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

// ******************************************************************************************************
//  XValueAxis.tsx - Gbtc
//
//  Copyright © 2021, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  10/22/2024 - Preston Crawford
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
var helper_functions_1 = __webpack_require__(/*! @gpa-gemstone/helper-functions */ "./node_modules/@gpa-gemstone/helper-functions/lib/index.js");
function XValueAxis(props) {
    var _a, _b, _c, _d;
    var context = React.useContext(GraphContext_1.GraphContext);
    var _e = __read(React.useState([]), 2), tick = _e[0], setTick = _e[1];
    var _f = __read(React.useState(0), 2), hLabel = _f[0], setHlabel = _f[1];
    var _g = __read(React.useState(0), 2), hAxis = _g[0], setHAxis = _g[1];
    var _h = __read(React.useState(props.label), 2), title = _h[0], setTitle = _h[1];
    var _j = __read(React.useState(0), 2), decimalPlaces = _j[0], setDecimalPlaces = _j[1];
    // Adjust space for X Axis labels
    React.useEffect(function () {
        setHlabel(title != null ? (0, helper_functions_1.GetTextHeight)('Segoe UI', '1em', title) : 0);
    }, [title]);
    // Adjust axis title
    React.useEffect(function () {
        var _a;
        setTitle((_a = props.label) !== null && _a !== void 0 ? _a : '');
    }, [props.label]);
    //Calculate ticks
    React.useEffect(function () {
        var deltaX = context.XDomain[1] - context.XDomain[0];
        if (deltaX === 0)
            return;
        var _a = calculateTickInterval(context.XDomain[0], context.XDomain[1]), tickInterval = _a.tickInterval, decimalPlaces = _a.decimalPlaces;
        var startTick = Math.ceil(context.XDomain[0] / tickInterval) * tickInterval;
        var endTick = context.XDomain[1];
        var newTicks = [];
        for (var t = startTick; t <= endTick; t += tickInterval) {
            newTicks.push(t);
        }
        setTick(newTicks);
        setDecimalPlaces(decimalPlaces);
    }, [context.XDomain]);
    //Adjust spacing for tick labels
    React.useEffect(function () {
        var maxLabelHeight = Math.max.apply(Math, __spreadArray([], __read(tick.map(function (t) { return (0, helper_functions_1.GetTextHeight)('Segoe UI', '1em', formatNumber(t, decimalPlaces)); })), false));
        var dX = (isFinite(maxLabelHeight) ? maxLabelHeight : 0) + 12;
        setHAxis(dX);
    }, [tick, decimalPlaces]);
    //Adjust axis height
    React.useEffect(function () {
        if (hAxis + hLabel !== props.heightAxis)
            props.setHeight(hAxis + hLabel);
    }, [hAxis, hLabel, props.heightAxis]);
    return (React.createElement("g", null,
        React.createElement("path", { stroke: "black", style: { strokeWidth: 1 }, d: "M ".concat(props.offsetLeft - (((_a = props.showLeftMostTick) !== null && _a !== void 0 ? _a : true) ? 0 : 8), " ").concat(props.height - props.offsetBottom, " H ").concat(props.width - props.offsetRight + (((_b = props.showRightMostTick) !== null && _b !== void 0 ? _b : true) ? 0 : 8)) }),
        ((_c = props.showLeftMostTick) !== null && _c !== void 0 ? _c : true) ? (React.createElement("path", { stroke: "black", style: { strokeWidth: 1 }, d: "M ".concat(props.offsetLeft, " ").concat(props.height - props.offsetBottom, " v 8") })) : null,
        ((_d = props.showRightMostTick) !== null && _d !== void 0 ? _d : true) ? (React.createElement("path", { stroke: "black", style: { strokeWidth: 1 }, d: "M ".concat(props.width - props.offsetRight, " ").concat(props.height - props.offsetBottom, " v 8") })) : null,
        (React.createElement(React.Fragment, null,
            tick.map(function (l, i) { return (React.createElement("path", { key: i, stroke: "black", style: { strokeWidth: 1, transition: 'd 0.5s' }, d: "M ".concat(context.XTransformation(l), " ").concat(props.height - props.offsetBottom + 6, " v -6") })); }),
            tick.map(function (l, i) { return (React.createElement("text", { fill: "black", key: i, style: { fontSize: '1em', textAnchor: 'middle', dominantBaseline: 'hanging', transition: 'x 0.5s, y 0.5s', }, y: props.height - props.offsetBottom + 8, x: context.XTransformation(l) }, formatNumber(l, decimalPlaces))); }))),
        title != null ? (React.createElement("text", { fill: "black", style: { fontSize: '1em', textAnchor: 'middle', dominantBaseline: 'middle' }, x: props.offsetLeft + (props.width - props.offsetLeft - props.offsetRight) / 2, y: props.height - props.offsetBottom + hAxis }, title)) : null));
}
//helper functions
var formatNumber = function (value, decimalPlaces) {
    return value.toFixed(decimalPlaces);
};
var calculateTickInterval = function (min, max) {
    var range = max - min;
    var desiredTicks = 7;
    var rawTickInterval = range / desiredTicks;
    var exponent = Math.floor(Math.log10(rawTickInterval));
    var fraction = rawTickInterval / Math.pow(10, exponent);
    var niceFraction;
    if (fraction <= 1)
        niceFraction = 1;
    else if (fraction <= 2)
        niceFraction = 2;
    else if (fraction <= 5)
        niceFraction = 5;
    else
        niceFraction = 10;
    var tickInterval = niceFraction * Math.pow(10, exponent);
    var decimalPlaces = Math.max(0, -Math.floor(Math.log10(tickInterval)));
    return { tickInterval: tickInterval, decimalPlaces: decimalPlaces };
};
exports["default"] = React.memo(XValueAxis);
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiWFZhbHVlQXhpcy5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uL3NyYy9YVmFsdWVBeGlzLnRzeCJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiO0FBQUEseUdBQXlHO0FBQ3pHLHlCQUF5QjtBQUN6QixFQUFFO0FBQ0YscUVBQXFFO0FBQ3JFLEVBQUU7QUFDRix3R0FBd0c7QUFDeEcsd0dBQXdHO0FBQ3hHLHNHQUFzRztBQUN0Ryx3RkFBd0Y7QUFDeEYsRUFBRTtBQUNGLDBDQUEwQztBQUMxQyxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4Ryw0RUFBNEU7QUFDNUUsRUFBRTtBQUNGLDhCQUE4QjtBQUM5Qix3R0FBd0c7QUFDeEcsaUNBQWlDO0FBQ2pDLG1EQUFtRDtBQUNuRCxFQUFFO0FBQ0YseUdBQXlHOzs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7QUFFekcsNkJBQStCO0FBQy9CLCtDQUE4QztBQUM5QyxtRUFBK0Q7QUFlL0QsU0FBUyxVQUFVLENBQUMsS0FBYTs7SUFDN0IsSUFBTSxPQUFPLEdBQUcsS0FBSyxDQUFDLFVBQVUsQ0FBQywyQkFBWSxDQUFDLENBQUM7SUFDekMsSUFBQSxLQUFBLE9BQWtCLEtBQUssQ0FBQyxRQUFRLENBQVcsRUFBRSxDQUFDLElBQUEsRUFBN0MsSUFBSSxRQUFBLEVBQUUsT0FBTyxRQUFnQyxDQUFDO0lBQy9DLElBQUEsS0FBQSxPQUFzQixLQUFLLENBQUMsUUFBUSxDQUFTLENBQUMsQ0FBQyxJQUFBLEVBQTlDLE1BQU0sUUFBQSxFQUFFLFNBQVMsUUFBNkIsQ0FBQztJQUNoRCxJQUFBLEtBQUEsT0FBb0IsS0FBSyxDQUFDLFFBQVEsQ0FBUyxDQUFDLENBQUMsSUFBQSxFQUE1QyxLQUFLLFFBQUEsRUFBRSxRQUFRLFFBQTZCLENBQUM7SUFDOUMsSUFBQSxLQUFBLE9BQW9CLEtBQUssQ0FBQyxRQUFRLENBQXFCLEtBQUssQ0FBQyxLQUFLLENBQUMsSUFBQSxFQUFsRSxLQUFLLFFBQUEsRUFBRSxR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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/YValueAxis.js":
/*!******************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/YValueAxis.js ***!
  \******************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {

"use strict";

// ******************************************************************************************************
//  ValueAxis.tsx - Gbtc
//
//  Copyright © 2021, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
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
//  03/19/2021 - C. lackner
//       Generated original version of source code.
//
// ******************************************************************************************************
var __read = (this && this.__read) || function (o, n) {
    var m = typeof Symbol === "function" && o[Symbol.iterator];
    if (!m) return o;
    var i = m.call(o), r, ar = [], e;
    try {
        while ((n === void 0 || n-- > 0) && !(r = i.next()).done) ar.push(r.value);
    }
    catch (error) { e = { error: error }; }
    finally {
        try {
            if (r && !r.done && (m = i["return"])) m.call(i);
        }
        finally { if (e) throw e.error; }
    }
    return ar;
};
var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
var React = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
var helper_functions_1 = __webpack_require__(/*! @gpa-gemstone/helper-functions */ "./node_modules/@gpa-gemstone/helper-functions/lib/index.js");
function ValueAxis(props) {
    var context = React.useContext(GraphContext_1.GraphContext);
    var _a = __read(React.useState([]), 2), tick = _a[0], setTick = _a[1];
    var _b = __read(React.useState(0), 2), hLabel = _b[0], setHlabel = _b[1];
    var _c = __read(React.useState(1), 2), ftSizeLabel = _c[0], setFtSizeLabel = _c[1];
    var _d = __read(React.useState(0), 2), hAxis = _d[0], setHAxis = _d[1];
    var _e = __read(React.useState(1), 2), nDigits = _e[0], setNdigits = _e[1];
    var _f = __read(React.useState(1), 2), factor = _f[0], setFactor = _f[1];
    React.useEffect(function () {
        var axis = GraphContext_1.AxisMap.get(props.axis);
        var dY = context.YDomain[axis][1] - context.YDomain[axis][0];
        if (!isFinite(dY) || isNaN(dY)) {
            setTick([]);
            return;
        }
        var newTicks;
        if (dY === 0) {
            newTicks = [context.YDomain[axis][0]];
        }
        else {
            var exp = 0;
            while ((dY * Math.pow(10, exp)) < 1) {
                exp = exp + 1;
            }
            while ((dY * Math.pow(10, exp)) > 10) {
                exp = exp - 1;
            }
            var scale = 1.0 / Math.pow(10, exp);
            if (dY * Math.pow(10, exp) < 6 && dY * Math.pow(10, exp) >= 2.5)
                scale = 0.5 / Math.pow(10, exp);
            if (dY * Math.pow(10, exp) < 2.5 && dY * Math.pow(10, exp) >= 1.2)
                scale = 0.2 / Math.pow(10, exp);
            if (dY * Math.pow(10, exp) < 1.2)
                scale = 0.1 / Math.pow(10, exp);
            var offset = Math.floor(context.YDomain[axis][0] / scale) * scale;
            newTicks = [offset + scale];
            while (newTicks[newTicks.length - 1] < (context.YDomain[axis][1] - scale))
                newTicks.push(newTicks[newTicks.length - 1] + scale);
        }
        var expF = 0;
        var Ymax = Math.max(Math.abs(context.YDomain[axis][0]), Math.abs(context.YDomain[axis][1]));
        while ((Ymax * Math.pow(10, expF)) < 1) {
            expF = expF + 1;
        }
        while ((Ymax * Math.pow(10, expF)) > 10) {
            expF = expF - 1;
        }
        expF = Math.sign(expF) * (Math.floor(Math.abs(expF) / 3)) * 3;
        // adjust to avoid same value on axis scenario
        if (dY * Math.pow(10, expF) < 0.1 && dY !== 0)
            expF = expF + 3;
        if (props.useFactor)
            setFactor(Math.pow(10, expF));
        else
            setFactor(1);
        setTick(newTicks);
    }, [context.YDomain, props.useFactor, props.axis]);
    React.useEffect(function () {
        var axis = GraphContext_1.AxisMap.get(props.axis);
        var dY = context.YDomain[axis][1] - context.YDomain[axis][0];
        dY = dY * factor;
        if (dY === 0)
            dY = Math.abs(context.YDomain[axis][0] * factor);
        if (dY >= 15)
            setNdigits(0);
        if (dY < 15 && dY >= 1.5)
            setNdigits(1);
        if (dY < 1.5 && dY >= 0.15)
            setNdigits(2);
        if (dY < 0.15)
            setNdigits(3);
        if (dY < 0.015)
            setNdigits(4);
        if (dY < 0.0015)
            setNdigits(5);
        if (dY === 0)
            setNdigits(2);
    }, [factor, context.YDomain, props.axis]);
    React.useEffect(function () {
        var h = 0;
        if (factor !== 1)
            h = (0, helper_functions_1.GetTextHeight)("Segoe UI", '1em', 'x' + (1 / factor).toString());
        if (h !== props.hFactor)
            props.setHeightFactor(h);
    }, [factor, props.hFactor, props.setHeightFactor]);
    React.useEffect(function () {
        if (props.label === undefined) {
            setHlabel(0);
            return;
        }
        var h = (0, helper_functions_1.GetTextHeight)("Segoe UI", ftSizeLabel + 'em', props.label) + 4;
        setHlabel(h);
    }, [props.label, props.height, props.offsetTop, props.offsetBottom, ftSizeLabel]);
    React.useEffect(function () {
        var dY = Math.max.apply(Math, __spreadArray([], __read(tick.map(function (t) { return (0, helper_functions_1.GetTextWidth)("Segoe UI", '1em', (t * factor).toFixed(nDigits)); })), false));
        dY = (isFinite(dY) ? dY : 0) + 8;
        if (dY !== hAxis)
            setHAxis(dY);
    }, [tick, nDigits]);
    React.useEffect(function () {
        if (props.hAxis !== hAxis + hLabel)
            props.setWidthAxis(hAxis + hLabel);
    }, [hAxis, hLabel, props.hAxis]);
    // use effect resets us in case this becomes unmounted
    React.useEffect(function () {
        return function () { return props.setWidthAxis(0); };
    }, []);
    React.useEffect(function () {
        if (props.label === undefined)
            return;
        var h = (0, helper_functions_1.GetTextWidth)("Segoe UI", '1em', props.label);
        var size = 1;
        while (h > props.height && size > 0.1) {
            size = size - 0.1;
            h = (0, helper_functions_1.GetTextWidth)("Segoe UI", size + 'em', props.label);
        }
        if (ftSizeLabel !== size)
            setFtSizeLabel(size);
    }, [props.label, props.height]);
    var leftPosition = React.useMemo(function () {
        if (props.axis === undefined || props.axis === 'left')
            return props.offsetLeft;
        else
            return props.width - props.offsetRight;
    }, [props.offsetLeft, props.offsetRight, props.width, props.axis]);
    var tickDirection = React.useMemo(function () {
        if (props.axis === undefined || props.axis === 'left')
            return -1;
        else
            return 1;
    }, [props.axis]);
    return (React.createElement("g", null,
        React.createElement("path", { stroke: 'black', style: { strokeWidth: 1, transition: 'd 0.5s' }, d: "M ".concat(leftPosition, " ").concat(props.height - props.offsetBottom + 8, " V ").concat(props.offsetTop) }),
        React.createElement("path", { stroke: 'black', style: { strokeWidth: 1, transition: 'd 0.5s' }, d: "M ".concat(leftPosition, " ").concat(props.offsetTop, " h ").concat(tickDirection * 8) }),
        tick.map(function (l, i) { var _a; return React.createElement("path", { key: i, stroke: ((props.axis === undefined || props.axis === 'left') ? 'lightgrey' : 'darkgrey'), strokeOpacity: ((_a = props.showGrid) !== null && _a !== void 0 ? _a : false) ? '0.8' : '0.0', style: { strokeWidth: 1, transition: 'd 0.5s' }, d: "M ".concat(props.offsetLeft, " ").concat(context.YTransformation(l, GraphContext_1.AxisMap.get(props.axis)), " h ").concat(props.width - props.offsetLeft - props.offsetRight) }); }),
        tick.map(function (l, i) { return React.createElement("path", { key: i, stroke: 'black', style: { strokeWidth: 1, transition: 'd 1s' }, d: "M ".concat(leftPosition, " ").concat(context.YTransformation(l, GraphContext_1.AxisMap.get(props.axis)), " h ").concat(tickDirection * 6) }); }),
        tick.map(function (l, i) { return React.createElement("text", { fill: 'black', key: i, style: { fontSize: '1em', textAnchor: (props.axis === undefined || props.axis === 'left') ? 'end' : 'start', transition: 'x 0.5s, y 0.5s' }, dominantBaseline: 'middle', x: leftPosition + tickDirection * 8, y: context.YTransformation(l, GraphContext_1.AxisMap.get(props.axis)) }, (l * factor).toFixed(nDigits)); }),
        props.label !== undefined ? React.createElement("text", { fill: 'black', style: { fontSize: ftSizeLabel + 'em', textAnchor: 'middle' }, dominantBaseline: 'text-bottom', transform: "rotate(".concat(tickDirection * 90, ",").concat(leftPosition + tickDirection * (hAxis + 4), ",").concat((props.offsetTop - props.offsetBottom + props.height) / 2.0, ")"), x: leftPosition + tickDirection * (hAxis + 4), y: (props.offsetTop - props.offsetBottom + props.height) / 2.0 }, props.label) : null,
        factor !== 1 ? React.createElement("text", { fill: 'black', style: { fontSize: '1em' }, x: leftPosition, y: props.offsetTop - 5 },
            "x",
            1 / factor) : null));
}
exports["default"] = React.memo(ValueAxis);
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiWVZhbHVlQXhpcy5qcyIsInNvdXJjZVJvb3QiOiIiLCJzb3VyY2VzIjpbIi4uL3NyYy9ZVmFsdWVBeGlzLnRzeCJdLCJuYW1lcyI6W10sIm1hcHBpbmdzIjoiO0FBQUMseUdBQXlHO0FBQzFHLHdCQUF3QjtBQUN4QixFQUFFO0FBQ0YscUVBQXFFO0FBQ3JFLEVBQUU7QUFDRix3R0FBd0c7QUFDeEcsd0dBQXdHO0FBQ3hHLHNHQUFzRztBQUN0Ryx3RkFBd0Y7QUFDeEYsRUFBRTtBQUNGLDBDQUEwQztBQUMxQyxFQUFFO0FBQ0Ysd0dBQXdHO0FBQ3hHLHdHQUF3RztBQUN4Ryw0RUFBNEU7QUFDNUUsRUFBRTtBQUNGLDhCQUE4QjtBQUM5Qix3R0FBd0c7QUFDeEcsMkJBQTJCO0FBQzNCLG1EQUFtRDtBQUNuRCxFQUFFO0FBQ0YseUdBQXlHOzs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7QUFFekcsNkJBQStCO0FBQy9CLCtDQUFvRTtBQUNwRSxtRUFBMkU7QUFtQjNFLFNBQVMsU0FBUyxDQUFDLEtBQWE7SUFDOUIsSUFBTSxPQUFPLEdBQUcsS0FBSyxDQUFDLFVBQVUsQ0FBQywyQkFBWSxDQUFDLENBQUE7SUFDeEMsSUFBQSxLQUFBLE9BQWlCLEtBQUssQ0FBQyxRQUFRLENBQVcsRUFBRSxDQUFDLElBQUEsRUFBNUMsSUFBSSxRQUFBLEVBQUMsT0FBTyxRQUFnQyxDQUFDO0lBRTlDLElBQUEsS0FBQSxPQUFzQixLQUFLLENBQUMsUUFBUSxDQUFTLENBQUMsQ0FBQyxJQUFBLEVBQTlDLE1BQU0sUUFBQSxFQUFFLFNBQVMsUUFBNkIsQ0FBQztJQUNoRCxJQUFBLEtBQUEsT0FBZ0MsS0FBSyxDQUFDLFFBQVEsQ0FBUyxDQUFDLENBQUMsSUFBQSxFQUF4RCxXQUFXLFFBQUEsRUFBRSxjQUFjLFFBQTZCLENBQUM7SUFDMUQsSUFBQSxLQUFBLE9BQW9CLEtBQUssQ0FBQyxRQUFRLENBQVMsQ0FBQyxDQUFDLElBQUEsRUFBNUMsS0FBSyxRQUFBLEVBQUUsUUFBUSxRQUE2QixD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

/***/ }),

/***/ "./node_modules/@gpa-gemstone/react-graph/lib/index.js":
/*!*************************************************************!*\
  !*** ./node_modules/@gpa-gemstone/react-graph/lib/index.js ***!
  \*************************************************************/
/***/ ((__unused_webpack_module, exports, __webpack_require__) => {

"use strict";

Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.HighlightBox = exports.AxisMap = exports.Infobox = exports.AggregatingCircles = exports.Oval = exports.Circle = exports.SymbolicMarker = exports.VerticalMarker = exports.HorizontalMarker = exports.Button = exports.LineWithThreshold = exports.HeatMapChart = exports.Line = exports.Plot = void 0;
var Plot_1 = __webpack_require__(/*! ./Plot */ "./node_modules/@gpa-gemstone/react-graph/lib/Plot.js");
exports.Plot = Plot_1.default;
var Line_1 = __webpack_require__(/*! ./Line */ "./node_modules/@gpa-gemstone/react-graph/lib/Line.js");
exports.Line = Line_1.default;
var HeatMapChart_1 = __webpack_require__(/*! ./HeatMapChart */ "./node_modules/@gpa-gemstone/react-graph/lib/HeatMapChart.js");
exports.HeatMapChart = HeatMapChart_1.default;
var LineWithThreshold_1 = __webpack_require__(/*! ./LineWithThreshold */ "./node_modules/@gpa-gemstone/react-graph/lib/LineWithThreshold.js");
exports.LineWithThreshold = LineWithThreshold_1.default;
var HorizontalMarker_1 = __webpack_require__(/*! ./HorizontalMarker */ "./node_modules/@gpa-gemstone/react-graph/lib/HorizontalMarker.js");
exports.HorizontalMarker = HorizontalMarker_1.default;
var VerticalMarker_1 = __webpack_require__(/*! ./VerticalMarker */ "./node_modules/@gpa-gemstone/react-graph/lib/VerticalMarker.js");
exports.VerticalMarker = VerticalMarker_1.default;
var SymbolicMarker_1 = __webpack_require__(/*! ./SymbolicMarker */ "./node_modules/@gpa-gemstone/react-graph/lib/SymbolicMarker.js");
exports.SymbolicMarker = SymbolicMarker_1.default;
var Button_1 = __webpack_require__(/*! ./Button */ "./node_modules/@gpa-gemstone/react-graph/lib/Button.js");
exports.Button = Button_1.default;
var AggregatingCircles_1 = __webpack_require__(/*! ./AggregatingCircles */ "./node_modules/@gpa-gemstone/react-graph/lib/AggregatingCircles.js");
exports.AggregatingCircles = AggregatingCircles_1.default;
var Circle_1 = __webpack_require__(/*! ./Circle */ "./node_modules/@gpa-gemstone/react-graph/lib/Circle.js");
exports.Circle = Circle_1.default;
var Infobox_1 = __webpack_require__(/*! ./Infobox */ "./node_modules/@gpa-gemstone/react-graph/lib/Infobox.js");
exports.Infobox = Infobox_1.default;
var Oval_1 = __webpack_require__(/*! ./Oval */ "./node_modules/@gpa-gemstone/react-graph/lib/Oval.js");
exports.Oval = Oval_1.default;
var GraphContext_1 = __webpack_require__(/*! ./GraphContext */ "./node_modules/@gpa-gemstone/react-graph/lib/GraphContext.js");
Object.defineProperty(exports, "AxisMap", ({ enumerable: true, get: function () { return GraphContext_1.AxisMap; } }));
var HighlightBox_1 = __webpack_require__(/*! ./HighlightBox */ "./node_modules/@gpa-gemstone/react-graph/lib/HighlightBox.js");
exports.HighlightBox = HighlightBox_1.default;
//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiaW5kZXguanMiLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyIuLi9zcmMvaW5kZXgudHMiXSwibmFtZXMiOltdLCJtYXBwaW5ncyI6Ijs7O0FBQUEsK0JBQTBCO0FBZ0J0QixlQWhCRyxjQUFJLENBZ0JIO0FBZlIsK0JBQTBCO0FBZ0J0QixlQWhCRyxjQUFJLENBZ0JIO0FBZlIsK0NBQTBDO0FBZ0J0Qyx1QkFoQkcsc0JBQVksQ0FnQkg7QUFmaEIseURBQW9EO0FBZ0JoRCw0QkFoQkcsMkJBQWlCLENBZ0JIO0FBZnJCLHVEQUFrRDtBQWlCOUMsMkJBakJHLDBCQUFnQixDQWlCSDtBQWhCcEIsbURBQThDO0FBaUIxQyx5QkFqQkcsd0JBQWMsQ0FpQkg7QUFoQmxCLG1EQUE4QztBQWlCMUMseUJBakJHLHdCQUFjLENBaUJIO0FBaEJsQixtQ0FBOEI7QUFhMUIsaUJBYkcsZ0JBQU0sQ0FhSDtBQVpWLDJEQUFzRDtBQWtCbEQsNkJBbEJHLDRCQUFrQixDQWtCSDtBQWpCdEIsbUNBQThCO0FBZTFCLGlCQWZHLGdCQUFNLENBZUg7QUFkVixxQ0FBZ0M7QUFpQjVCLGtCQWpCRyxpQkFBTyxDQWlCSDtBQWhCWCwrQkFBMEI7QUFjdEIsZUFkRyxjQUFJLENBY0g7QUFiUiwrQ0FBeUM7QUFnQnJDLHdGQWhCSyxzQkFBTyxPQWdCTDtBQWZYLCtDQUEwQztBQWdCdEMsdUJBaEJHLHNCQUFZLENBZ0JIIn0=

/***/ }),

/***/ "./node_modules/html2canvas/dist/html2canvas.js":
/*!******************************************************!*\
  !*** ./node_modules/html2canvas/dist/html2canvas.js ***!
  \******************************************************/
/***/ (function(module, __unused_webpack_exports, __webpack_require__) {

/* provided dependency */ var console = __webpack_require__(/*! console-browserify */ "./node_modules/console-browserify/index.js");
/*!
 * html2canvas 1.4.1 <https://html2canvas.hertzen.com>
 * Copyright (c) 2022 Niklas von Hertzen <https://hertzen.com>
 * Released under MIT License
 */
(function (global, factory) {
     true ? module.exports = factory() :
    0;
}(this, (function () { 'use strict';

    /*! *****************************************************************************
    Copyright (c) Microsoft Corporation.

    Permission to use, copy, modify, and/or distribute this software for any
    purpose with or without fee is hereby granted.

    THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH
    REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY
    AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
    INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM
    LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR
    OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
    PERFORMANCE OF THIS SOFTWARE.
    ***************************************************************************** */
    /* global Reflect, Promise */

    var extendStatics = function(d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };

    function __extends(d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    }

    var __assign = function() {
        __assign = Object.assign || function __assign(t) {
            for (var s, i = 1, n = arguments.length; i < n; i++) {
                s = arguments[i];
                for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p)) t[p] = s[p];
            }
            return t;
        };
        return __assign.apply(this, arguments);
    };

    function __awaiter(thisArg, _arguments, P, generator) {
        function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
        return new (P || (P = Promise))(function (resolve, reject) {
            function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
            function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
            function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
            step((generator = generator.apply(thisArg, _arguments || [])).next());
        });
    }

    function __generator(thisArg, body) {
        var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
        return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
        function verb(n) { return function (v) { return step([n, v]); }; }
        function step(op) {
            if (f) throw new TypeError("Generator is already executing.");
            while (_) try {
                if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
                if (y = 0, t) op = [op[0] & 2, t.value];
                switch (op[0]) {
                    case 0: case 1: t = op; break;
                    case 4: _.label++; return { value: op[1], done: false };
                    case 5: _.label++; y = op[1]; op = [0]; continue;
                    case 7: op = _.ops.pop(); _.trys.pop(); continue;
                    default:
                        if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                        if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                        if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                        if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                        if (t[2]) _.ops.pop();
                        _.trys.pop(); continue;
                }
                op = body.call(thisArg, _);
            } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
            if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
        }
    }

    function __spreadArray(to, from, pack) {
        if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
            if (ar || !(i in from)) {
                if (!ar) ar = Array.prototype.slice.call(from, 0, i);
                ar[i] = from[i];
            }
        }
        return to.concat(ar || from);
    }

    var Bounds = /** @class */ (function () {
        function Bounds(left, top, width, height) {
            this.left = left;
            this.top = top;
            this.width = width;
            this.height = height;
        }
        Bounds.prototype.add = function (x, y, w, h) {
            return new Bounds(this.left + x, this.top + y, this.width + w, this.height + h);
        };
        Bounds.fromClientRect = function (context, clientRect) {
            return new Bounds(clientRect.left + context.windowBounds.left, clientRect.top + context.windowBounds.top, clientRect.width, clientRect.height);
        };
        Bounds.fromDOMRectList = function (context, domRectList) {
            var domRect = Array.from(domRectList).find(function (rect) { return rect.width !== 0; });
            return domRect
                ? new Bounds(domRect.left + context.windowBounds.left, domRect.top + context.windowBounds.top, domRect.width, domRect.height)
                : Bounds.EMPTY;
        };
        Bounds.EMPTY = new Bounds(0, 0, 0, 0);
        return Bounds;
    }());
    var parseBounds = function (context, node) {
        return Bounds.fromClientRect(context, node.getBoundingClientRect());
    };
    var parseDocumentSize = function (document) {
        var body = document.body;
        var documentElement = document.documentElement;
        if (!body || !documentElement) {
            throw new Error("Unable to get document size");
        }
        var width = Math.max(Math.max(body.scrollWidth, documentElement.scrollWidth), Math.max(body.offsetWidth, documentElement.offsetWidth), Math.max(body.clientWidth, documentElement.clientWidth));
        var height = Math.max(Math.max(body.scrollHeight, documentElement.scrollHeight), Math.max(body.offsetHeight, documentElement.offsetHeight), Math.max(body.clientHeight, documentElement.clientHeight));
        return new Bounds(0, 0, width, height);
    };

    /*
     * css-line-break 2.1.0 <https://github.com/niklasvh/css-line-break#readme>
     * Copyright (c) 2022 Niklas von Hertzen <https://hertzen.com>
     * Released under MIT License
     */
    var toCodePoints$1 = function (str) {
        var codePoints = [];
        var i = 0;
        var length = str.length;
        while (i < length) {
            var value = str.charCodeAt(i++);
            if (value >= 0xd800 && value <= 0xdbff && i < length) {
                var extra = str.charCodeAt(i++);
                if ((extra & 0xfc00) === 0xdc00) {
                    codePoints.push(((value & 0x3ff) << 10) + (extra & 0x3ff) + 0x10000);
                }
                else {
                    codePoints.push(value);
                    i--;
                }
            }
            else {
                codePoints.push(value);
            }
        }
        return codePoints;
    };
    var fromCodePoint$1 = function () {
        var codePoints = [];
        for (var _i = 0; _i < arguments.length; _i++) {
            codePoints[_i] = arguments[_i];
        }
        if (String.fromCodePoint) {
            return String.fromCodePoint.apply(String, codePoints);
        }
        var length = codePoints.length;
        if (!length) {
            return '';
        }
        var codeUnits = [];
        var index = -1;
        var result = '';
        while (++index < length) {
            var codePoint = codePoints[index];
            if (codePoint <= 0xffff) {
                codeUnits.push(codePoint);
            }
            else {
                codePoint -= 0x10000;
                codeUnits.push((codePoint >> 10) + 0xd800, (codePoint % 0x400) + 0xdc00);
            }
            if (index + 1 === length || codeUnits.length > 0x4000) {
                result += String.fromCharCode.apply(String, codeUnits);
                codeUnits.length = 0;
            }
        }
        return result;
    };
    var chars$2 = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
    // Use a lookup table to find the index.
    var lookup$2 = typeof Uint8Array === 'undefined' ? [] : new Uint8Array(256);
    for (var i$2 = 0; i$2 < chars$2.length; i$2++) {
        lookup$2[chars$2.charCodeAt(i$2)] = i$2;
    }

    /*
     * utrie 1.0.2 <https://github.com/niklasvh/utrie>
     * Copyright (c) 2022 Niklas von Hertzen <https://hertzen.com>
     * Released under MIT License
     */
    var chars$1$1 = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
    // Use a lookup table to find the index.
    var lookup$1$1 = typeof Uint8Array === 'undefined' ? [] : new Uint8Array(256);
    for (var i$1$1 = 0; i$1$1 < chars$1$1.length; i$1$1++) {
        lookup$1$1[chars$1$1.charCodeAt(i$1$1)] = i$1$1;
    }
    var decode$1 = function (base64) {
        var bufferLength = base64.length * 0.75, len = base64.length, i, p = 0, encoded1, encoded2, encoded3, encoded4;
        if (base64[base64.length - 1] === '=') {
            bufferLength--;
            if (base64[base64.length - 2] === '=') {
                bufferLength--;
            }
        }
        var buffer = typeof ArrayBuffer !== 'undefined' &&
            typeof Uint8Array !== 'undefined' &&
            typeof Uint8Array.prototype.slice !== 'undefined'
            ? new ArrayBuffer(bufferLength)
            : new Array(bufferLength);
        var bytes = Array.isArray(buffer) ? buffer : new Uint8Array(buffer);
        for (i = 0; i < len; i += 4) {
            encoded1 = lookup$1$1[base64.charCodeAt(i)];
            encoded2 = lookup$1$1[base64.charCodeAt(i + 1)];
            encoded3 = lookup$1$1[base64.charCodeAt(i + 2)];
            encoded4 = lookup$1$1[base64.charCodeAt(i + 3)];
            bytes[p++] = (encoded1 << 2) | (encoded2 >> 4);
            bytes[p++] = ((encoded2 & 15) << 4) | (encoded3 >> 2);
            bytes[p++] = ((encoded3 & 3) << 6) | (encoded4 & 63);
        }
        return buffer;
    };
    var polyUint16Array$1 = function (buffer) {
        var length = buffer.length;
        var bytes = [];
        for (var i = 0; i < length; i += 2) {
            bytes.push((buffer[i + 1] << 8) | buffer[i]);
        }
        return bytes;
    };
    var polyUint32Array$1 = function (buffer) {
        var length = buffer.length;
        var bytes = [];
        for (var i = 0; i < length; i += 4) {
            bytes.push((buffer[i + 3] << 24) | (buffer[i + 2] << 16) | (buffer[i + 1] << 8) | buffer[i]);
        }
        return bytes;
    };

    /** Shift size for getting the index-2 table offset. */
    var UTRIE2_SHIFT_2$1 = 5;
    /** Shift size for getting the index-1 table offset. */
    var UTRIE2_SHIFT_1$1 = 6 + 5;
    /**
     * Shift size for shifting left the index array values.
     * Increases possible data size with 16-bit index values at the cost
     * of compactability.
     * This requires data blocks to be aligned by UTRIE2_DATA_GRANULARITY.
     */
    var UTRIE2_INDEX_SHIFT$1 = 2;
    /**
     * Difference between the two shift sizes,
     * for getting an index-1 offset from an index-2 offset. 6=11-5
     */
    var UTRIE2_SHIFT_1_2$1 = UTRIE2_SHIFT_1$1 - UTRIE2_SHIFT_2$1;
    /**
     * The part of the index-2 table for U+D800..U+DBFF stores values for
     * lead surrogate code _units_ not code _points_.
     * Values for lead surrogate code _points_ are indexed with this portion of the table.
     * Length=32=0x20=0x400>>UTRIE2_SHIFT_2. (There are 1024=0x400 lead surrogates.)
     */
    var UTRIE2_LSCP_INDEX_2_OFFSET$1 = 0x10000 >> UTRIE2_SHIFT_2$1;
    /** Number of entries in a data block. 32=0x20 */
    var UTRIE2_DATA_BLOCK_LENGTH$1 = 1 << UTRIE2_SHIFT_2$1;
    /** Mask for getting the lower bits for the in-data-block offset. */
    var UTRIE2_DATA_MASK$1 = UTRIE2_DATA_BLOCK_LENGTH$1 - 1;
    var UTRIE2_LSCP_INDEX_2_LENGTH$1 = 0x400 >> UTRIE2_SHIFT_2$1;
    /** Count the lengths of both BMP pieces. 2080=0x820 */
    var UTRIE2_INDEX_2_BMP_LENGTH$1 = UTRIE2_LSCP_INDEX_2_OFFSET$1 + UTRIE2_LSCP_INDEX_2_LENGTH$1;
    /**
     * The 2-byte UTF-8 version of the index-2 table follows at offset 2080=0x820.
     * Length 32=0x20 for lead bytes C0..DF, regardless of UTRIE2_SHIFT_2.
     */
    var UTRIE2_UTF8_2B_INDEX_2_OFFSET$1 = UTRIE2_INDEX_2_BMP_LENGTH$1;
    var UTRIE2_UTF8_2B_INDEX_2_LENGTH$1 = 0x800 >> 6; /* U+0800 is the first code point after 2-byte UTF-8 */
    /**
     * The index-1 table, only used for supplementary code points, at offset 2112=0x840.
     * Variable length, for code points up to highStart, where the last single-value range starts.
     * Maximum length 512=0x200=0x100000>>UTRIE2_SHIFT_1.
     * (For 0x100000 supplementary code points U+10000..U+10ffff.)
     *
     * The part of the index-2 table for supplementary code points starts
     * after this index-1 table.
     *
     * Both the index-1 table and the following part of the index-2 table
     * are omitted completely if there is only BMP data.
     */
    var UTRIE2_INDEX_1_OFFSET$1 = UTRIE2_UTF8_2B_INDEX_2_OFFSET$1 + UTRIE2_UTF8_2B_INDEX_2_LENGTH$1;
    /**
     * Number of index-1 entries for the BMP. 32=0x20
     * This part of the index-1 table is omitted from the serialized form.
     */
    var UTRIE2_OMITTED_BMP_INDEX_1_LENGTH$1 = 0x10000 >> UTRIE2_SHIFT_1$1;
    /** Number of entries in an index-2 block. 64=0x40 */
    var UTRIE2_INDEX_2_BLOCK_LENGTH$1 = 1 << UTRIE2_SHIFT_1_2$1;
    /** Mask for getting the lower bits for the in-index-2-block offset. */
    var UTRIE2_INDEX_2_MASK$1 = UTRIE2_INDEX_2_BLOCK_LENGTH$1 - 1;
    var slice16$1 = function (view, start, end) {
        if (view.slice) {
            return view.slice(start, end);
        }
        return new Uint16Array(Array.prototype.slice.call(view, start, end));
    };
    var slice32$1 = function (view, start, end) {
        if (view.slice) {
            return view.slice(start, end);
        }
        return new Uint32Array(Array.prototype.slice.call(view, start, end));
    };
    var createTrieFromBase64$1 = function (base64, _byteLength) {
        var buffer = decode$1(base64);
        var view32 = Array.isArray(buffer) ? polyUint32Array$1(buffer) : new Uint32Array(buffer);
        var view16 = Array.isArray(buffer) ? polyUint16Array$1(buffer) : new Uint16Array(buffer);
        var headerLength = 24;
        var index = slice16$1(view16, headerLength / 2, view32[4] / 2);
        var data = view32[5] === 2
            ? slice16$1(view16, (headerLength + view32[4]) / 2)
            : slice32$1(view32, Math.ceil((headerLength + view32[4]) / 4));
        return new Trie$1(view32[0], view32[1], view32[2], view32[3], index, data);
    };
    var Trie$1 = /** @class */ (function () {
        function Trie(initialValue, errorValue, highStart, highValueIndex, index, data) {
            this.initialValue = initialValue;
            this.errorValue = errorValue;
            this.highStart = highStart;
            this.highValueIndex = highValueIndex;
            this.index = index;
            this.data = data;
        }
        /**
         * Get the value for a code point as stored in the Trie.
         *
         * @param codePoint the code point
         * @return the value
         */
        Trie.prototype.get = function (codePoint) {
            var ix;
            if (codePoint >= 0) {
                if (codePoint < 0x0d800 || (codePoint > 0x0dbff && codePoint <= 0x0ffff)) {
                    // Ordinary BMP code point, excluding leading surrogates.
                    // BMP uses a single level lookup.  BMP index starts at offset 0 in the Trie2 index.
                    // 16 bit data is stored in the index array itself.
                    ix = this.index[codePoint >> UTRIE2_SHIFT_2$1];
                    ix = (ix << UTRIE2_INDEX_SHIFT$1) + (codePoint & UTRIE2_DATA_MASK$1);
                    return this.data[ix];
                }
                if (codePoint <= 0xffff) {
                    // Lead Surrogate Code Point.  A Separate index section is stored for
                    // lead surrogate code units and code points.
                    //   The main index has the code unit data.
                    //   For this function, we need the code point data.
                    // Note: this expression could be refactored for slightly improved efficiency, but
                    //       surrogate code points will be so rare in practice that it's not worth it.
                    ix = this.index[UTRIE2_LSCP_INDEX_2_OFFSET$1 + ((codePoint - 0xd800) >> UTRIE2_SHIFT_2$1)];
                    ix = (ix << UTRIE2_INDEX_SHIFT$1) + (codePoint & UTRIE2_DATA_MASK$1);
                    return this.data[ix];
                }
                if (codePoint < this.highStart) {
                    // Supplemental code point, use two-level lookup.
                    ix = UTRIE2_INDEX_1_OFFSET$1 - UTRIE2_OMITTED_BMP_INDEX_1_LENGTH$1 + (codePoint >> UTRIE2_SHIFT_1$1);
                    ix = this.index[ix];
                    ix += (codePoint >> UTRIE2_SHIFT_2$1) & UTRIE2_INDEX_2_MASK$1;
                    ix = this.index[ix];
                    ix = (ix << UTRIE2_INDEX_SHIFT$1) + (codePoint & UTRIE2_DATA_MASK$1);
                    return this.data[ix];
                }
                if (codePoint <= 0x10ffff) {
                    return this.data[this.highValueIndex];
                }
            }
            // Fall through.  The code point is outside of the legal range of 0..0x10ffff.
            return this.errorValue;
        };
        return Trie;
    }());

    /*
     * base64-arraybuffer 1.0.2 <https://github.com/niklasvh/base64-arraybuffer>
     * Copyright (c) 2022 Niklas von Hertzen <https://hertzen.com>
     * Released under MIT License
     */
    var chars$3 = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
    // Use a lookup table to find the index.
    var lookup$3 = typeof Uint8Array === 'undefined' ? [] : new Uint8Array(256);
    for (var i$3 = 0; i$3 < chars$3.length; i$3++) {
        lookup$3[chars$3.charCodeAt(i$3)] = i$3;
    }

    var base64$1 = 'KwAAAAAAAAAACA4AUD0AADAgAAACAAAAAAAIABAAGABAAEgAUABYAGAAaABgAGgAYgBqAF8AZwBgAGgAcQB5AHUAfQCFAI0AlQCdAKIAqgCyALoAYABoAGAAaABgAGgAwgDKAGAAaADGAM4A0wDbAOEA6QDxAPkAAQEJAQ8BFwF1AH0AHAEkASwBNAE6AUIBQQFJAVEBWQFhAWgBcAF4ATAAgAGGAY4BlQGXAZ8BpwGvAbUBvQHFAc0B0wHbAeMB6wHxAfkBAQIJAvEBEQIZAiECKQIxAjgCQAJGAk4CVgJeAmQCbAJ0AnwCgQKJApECmQKgAqgCsAK4ArwCxAIwAMwC0wLbAjAA4wLrAvMC+AIAAwcDDwMwABcDHQMlAy0DNQN1AD0DQQNJA0kDSQNRA1EDVwNZA1kDdQB1AGEDdQBpA20DdQN1AHsDdQCBA4kDkQN1AHUAmQOhA3UAdQB1AHUAdQB1AHUAdQB1AHUAdQB1AHUAdQB1AHUAdQB1AKYDrgN1AHUAtgO+A8YDzgPWAxcD3gPjA+sD8wN1AHUA+wMDBAkEdQANBBUEHQQlBCoEFwMyBDgEYABABBcDSARQBFgEYARoBDAAcAQzAXgEgASIBJAEdQCXBHUAnwSnBK4EtgS6BMIEyAR1AHUAdQB1AHUAdQCVANAEYABgAGAAYABgAGAAYABgANgEYADcBOQEYADsBPQE/AQEBQwFFAUcBSQFLAU0BWQEPAVEBUsFUwVbBWAAYgVgAGoFcgV6BYIFigWRBWAAmQWfBaYFYABgAGAAYABgAKoFYACxBbAFuQW6BcEFwQXHBcEFwQXPBdMF2wXjBeoF8gX6BQIGCgYSBhoGIgYqBjIGOgZgAD4GRgZMBmAAUwZaBmAAYABgAGAAYABgAGAAYABgAGAAYABgAGIGYABpBnAGYABgAGAAYABgAGAAYABgAGAAYAB4Bn8GhQZgAGAAYAB1AHcDFQSLBmAAYABgAJMGdQA9A3UAmwajBqsGqwaVALMGuwbDBjAAywbSBtIG1QbSBtIG0gbSBtIG0gbdBuMG6wbzBvsGAwcLBxMHAwcbByMHJwcsBywHMQcsB9IGOAdAB0gHTgfSBkgHVgfSBtIG0gbSBtIG0gbSBtIG0gbSBiwHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAdgAGAALAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAdbB2MHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsB2kH0gZwB64EdQB1AHUAdQB1AHUAdQB1AHUHfQdgAIUHjQd1AHUAlQedB2AAYAClB6sHYACzB7YHvgfGB3UAzgfWBzMB3gfmB1EB7gf1B/0HlQENAQUIDQh1ABUIHQglCBcDLQg1CD0IRQhNCEEDUwh1AHUAdQBbCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIaQhjCGQIZQhmCGcIaAhpCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIaQhjCGQIZQhmCGcIaAhpCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIaQhjCGQIZQhmCGcIaAhpCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIaQhjCGQIZQhmCGcIaAhpCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIaQhjCGQIZQhmCGcIaAhpCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIaQhjCGQIZQhmCGcIaAhpCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIaQhjCGQIZQhmCGcIaAhpCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIaQhjCGQIZQhmCGcIaAhpCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIaQhjCGQIZQhmCGcIaAhpCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIaQhjCGQIZQhmCGcIaAhpCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIaQhjCGQIZQhmCGcIaAhpCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIaQhjCGQIZQhmCGcIaAhpCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIaQhjCGQIZQhmCGcIaAhpCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIaQhjCGQIZQhmCGcIaAhpCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIaQhjCGQIZQhmCGcIaAhpCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIaQhjCGQIZQhmCGcIaAhpCGMIZAhlCGYIZwhoCGkIYwhkCGUIZghnCGgIcAh3CHoIMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwAIIIggiCCIIIggiCCIIIggiCCIIIggiCCIIIggiCCIIIggiCCIIIggiCCIIIggiCCIIIggiCCIIIggiCCIIIgggwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAALAcsBywHLAcsBywHLAcsBywHLAcsB4oILAcsB44I0gaWCJ4Ipgh1AHUAqgiyCHUAdQB1AHUAdQB1AHUAdQB1AHUAtwh8AXUAvwh1AMUIyQjRCNkI4AjoCHUAdQB1AO4I9gj+CAYJDgkTCS0HGwkjCYIIggiCCIIIggiCCIIIggiCCIIIggiCCIIIggiCCIIIggiCCIIIggiCCIIIggiCCIIIggiCCIIIggiCCIIIggiAAIAAAAFAAYABgAGIAXwBgAHEAdQBFAJUAogCyAKAAYABgAEIA4ABGANMA4QDxAMEBDwE1AFwBLAE6AQEBUQF4QkhCmEKoQrhCgAHIQsAB0MLAAcABwAHAAeDC6ABoAHDCwMMAAcABwAHAAdDDGMMAAcAB6MM4wwjDWMNow3jDaABoAGgAaABoAGgAaABoAGgAaABoAGgAaABoAGgAaABoAGgAaABoAEjDqABWw6bDqABpg6gAaABoAHcDvwOPA+gAaABfA/8DvwO/A78DvwO/A78DvwO/A78DvwO/A78DvwO/A78DvwO/A78DvwO/A78DvwO/A78DvwO/A78DpcPAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcAB9cPKwkyCToJMAB1AHUAdQBCCUoJTQl1AFUJXAljCWcJawkwADAAMAAwAHMJdQB2CX4JdQCECYoJjgmWCXUAngkwAGAAYABxAHUApgn3A64JtAl1ALkJdQDACTAAMAAwADAAdQB1AHUAdQB1AHUAdQB1AHUAowYNBMUIMAAwADAAMADICcsJ0wnZCRUE4QkwAOkJ8An4CTAAMAB1AAAKvwh1AAgKDwoXCh8KdQAwACcKLgp1ADYKqAmICT4KRgowADAAdQB1AE4KMAB1AFYKdQBeCnUAZQowADAAMAAwADAAMAAwADAAMAAVBHUAbQowADAAdQC5CXUKMAAwAHwBxAijBogEMgF9CoQKiASMCpQKmgqIBKIKqgquCogEDQG2Cr4KxgrLCjAAMADTCtsKCgHjCusK8Qr5CgELMAAwADAAMAB1AIsECQsRC3UANAEZCzAAMAAwADAAMAB1ACELKQswAHUANAExCzkLdQBBC0kLMABRC1kLMAAwADAAMAAwADAAdQBhCzAAMAAwAGAAYABpC3ELdwt/CzAAMACHC4sLkwubC58Lpwt1AK4Ltgt1APsDMAAwADAAMAAwADAAMAAwAL4LwwvLC9IL1wvdCzAAMADlC+kL8Qv5C/8LSQswADAAMAAwADAAMAAwADAAMAAHDDAAMAAwADAAMAAODBYMHgx1AHUAdQB1AHUAdQB1AHUAdQB1AHUAdQB1AHUAdQB1AHUAdQB1AHUAdQB1AHUAdQB1AHUAdQB1ACYMMAAwADAAdQB1AHUALgx1AHUAdQB1AHUAdQA2DDAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwAHUAdQB1AHUAdQB1AHUAdQB1AHUAdQB1AHUAdQB1AHUAdQB1AD4MdQBGDHUAdQB1AHUAdQB1AEkMdQB1AHUAdQB1AFAMMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwAHUAdQB1AHUAdQB1AHUAdQB1AHUAdQB1AHUAdQBYDHUAdQB1AF8MMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAB1AHUAdQB1AHUAdQB1AHUAdQB1AHUAdQB1AHUAdQB1AHUA+wMVBGcMMAAwAHwBbwx1AHcMfwyHDI8MMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAYABgAJcMMAAwADAAdQB1AJ8MlQClDDAAMACtDCwHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsB7UMLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHdQB1AHUAdQB1AHUAdQB1AHUAdQB1AHUAdQB1AA0EMAC9DDAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAsBywHLAcsBywHLAcsBywHLQcwAMEMyAwsBywHLAcsBywHLAcsBywHLAcsBywHzAwwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwAHUAdQB1ANQM2QzhDDAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMABgAGAAYABgAGAAYABgAOkMYADxDGAA+AwADQYNYABhCWAAYAAODTAAMAAwADAAFg1gAGAAHg37AzAAMAAwADAAYABgACYNYAAsDTQNPA1gAEMNPg1LDWAAYABgAGAAYABgAGAAYABgAGAAUg1aDYsGVglhDV0NcQBnDW0NdQ15DWAAYABgAGAAYABgAGAAYABgAGAAYABgAGAAYABgAGAAlQCBDZUAiA2PDZcNMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAnw2nDTAAMAAwADAAMAAwAHUArw23DTAAMAAwADAAMAAwADAAMAAwADAAMAB1AL8NMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAB1AHUAdQB1AHUAdQDHDTAAYABgAM8NMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAA1w11ANwNMAAwAD0B5A0wADAAMAAwADAAMADsDfQN/A0EDgwOFA4wABsOMAAwADAAMAAwADAAMAAwANIG0gbSBtIG0gbSBtIG0gYjDigOwQUuDsEFMw7SBjoO0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIGQg5KDlIOVg7SBtIGXg5lDm0OdQ7SBtIGfQ6EDooOjQ6UDtIGmg6hDtIG0gaoDqwO0ga0DrwO0gZgAGAAYADEDmAAYAAkBtIGzA5gANIOYADaDokO0gbSBt8O5w7SBu8O0gb1DvwO0gZgAGAAxA7SBtIG0gbSBtIGYABgAGAAYAAED2AAsAUMD9IG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIGFA8sBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAccD9IGLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHJA8sBywHLAcsBywHLAccDywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywPLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAc0D9IG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIGLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAccD9IG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIGFA8sBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHLAcsBywHPA/SBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gbSBtIG0gYUD0QPlQCVAJUAMAAwADAAMACVAJUAlQCVAJUAlQCVAEwPMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAA//8EAAQABAAEAAQABAAEAAQABAANAAMAAQABAAIABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQACgATABcAHgAbABoAHgAXABYAEgAeABsAGAAPABgAHABLAEsASwBLAEsASwBLAEsASwBLABgAGAAeAB4AHgATAB4AUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQABYAGwASAB4AHgAeAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAWAA0AEQAeAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArAAQABAAEAAQABAAFAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAJABYAGgAbABsAGwAeAB0AHQAeAE8AFwAeAA0AHgAeABoAGwBPAE8ADgBQAB0AHQAdAE8ATwAXAE8ATwBPABYAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAB0AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAdAFAAUABQAFAAUABQAFAAUAAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAFAAHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAeAB4AHgAeAFAATwBAAE8ATwBPAEAATwBQAFAATwBQAB4AHgAeAB4AHgAeAB0AHQAdAB0AHgAdAB4ADgBQAFAAUABQAFAAHgAeAB4AHgAeAB4AHgBQAB4AUAAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4ABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAJAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAkACQAJAAkACQAJAAkABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAeAB4AHgAeAFAAHgAeAB4AKwArAFAAUABQAFAAGABQACsAKwArACsAHgAeAFAAHgBQAFAAUAArAFAAKwAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AKwAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4ABAAEAAQABAAEAAQABAAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAUAAeAB4AHgAeAB4AHgBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAYAA0AKwArAB4AHgAbACsABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQADQAEAB4ABAAEAB4ABAAEABMABAArACsAKwArACsAKwArACsAVgBWAFYAVgBWAFYAVgBWAFYAVgBWAFYAVgBWAFYAVgBWAFYAVgBWAFYAVgBWAFYAVgBWAFYAKwArACsAKwBWAFYAVgBWAB4AHgArACsAKwArACsAKwArACsAKwArACsAHgAeAB4AHgAeAB4AHgAeAB4AGgAaABoAGAAYAB4AHgAEAAQABAAEAAQABAAEAAQABAAEAAQAEwAEACsAEwATAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABABLAEsASwBLAEsASwBLAEsASwBLABoAGQAZAB4AUABQAAQAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQABMAUAAEAAQABAAEAAQABAAEAB4AHgAEAAQABAAEAAQABABQAFAABAAEAB4ABAAEAAQABABQAFAASwBLAEsASwBLAEsASwBLAEsASwBQAFAAUAAeAB4AUAAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AKwAeAFAABABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAABAAEAAQABAAEAAQABAAEAAQABAAEAFAAKwArACsAKwArACsAKwArACsAKwArACsAKwArAEsASwBLAEsASwBLAEsASwBLAEsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAABAAEAAQABAAEAAQABAAEAAQAUABQAB4AHgAYABMAUAArACsABAAbABsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAAEAFAABAAEAAQABAAEAFAABAAEAAQAUAAEAAQABAAEAAQAKwArAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAArACsAHgArAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArAFAAUABQAFAAUABQAFAAUABQAFAAKwArACsAKwArACsAKwArACsAKwArAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAB4ABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAAQABAAEAFAABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQAUAAEAAQABAAEAAQABAAEAFAAUABQAFAAUABQAFAAUABQAFAABAAEAA0ADQBLAEsASwBLAEsASwBLAEsASwBLAB4AUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAArAFAAUABQAFAAUABQAFAAUAArACsAUABQACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwBQAFAAUABQAFAAUABQACsAUAArACsAKwBQAFAAUABQACsAKwAEAFAABAAEAAQABAAEAAQABAArACsABAAEACsAKwAEAAQABABQACsAKwArACsAKwArACsAKwAEACsAKwArACsAUABQACsAUABQAFAABAAEACsAKwBLAEsASwBLAEsASwBLAEsASwBLAFAAUAAaABoAUABQAFAAUABQAEwAHgAbAFAAHgAEACsAKwAEAAQABAArAFAAUABQAFAAUABQACsAKwArACsAUABQACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwBQAFAAUABQAFAAUABQACsAUABQACsAUABQACsAUABQACsAKwAEACsABAAEAAQABAAEACsAKwArACsABAAEACsAKwAEAAQABAArACsAKwAEACsAKwArACsAKwArACsAUABQAFAAUAArAFAAKwArACsAKwArACsAKwBLAEsASwBLAEsASwBLAEsASwBLAAQABABQAFAAUAAEAB4AKwArACsAKwArACsAKwArACsAKwAEAAQABAArAFAAUABQAFAAUABQAFAAUABQACsAUABQAFAAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwBQAFAAUABQAFAAUABQACsAUABQACsAUABQAFAAUABQACsAKwAEAFAABAAEAAQABAAEAAQABAAEACsABAAEAAQAKwAEAAQABAArACsAUAArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwBQAFAABAAEACsAKwBLAEsASwBLAEsASwBLAEsASwBLAB4AGwArACsAKwArACsAKwArAFAABAAEAAQABAAEAAQAKwAEAAQABAArAFAAUABQAFAAUABQAFAAUAArACsAUABQACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAAQABAAEAAQABAArACsABAAEACsAKwAEAAQABAArACsAKwArACsAKwArAAQABAAEACsAKwArACsAUABQACsAUABQAFAABAAEACsAKwBLAEsASwBLAEsASwBLAEsASwBLAB4AUABQAFAAUABQAFAAUAArACsAKwArACsAKwArACsAKwArAAQAUAArAFAAUABQAFAAUABQACsAKwArAFAAUABQACsAUABQAFAAUAArACsAKwBQAFAAKwBQACsAUABQACsAKwArAFAAUAArACsAKwBQAFAAUAArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwArAAQABAAEAAQABAArACsAKwAEAAQABAArAAQABAAEAAQAKwArAFAAKwArACsAKwArACsABAArACsAKwArACsAKwArACsAKwArAEsASwBLAEsASwBLAEsASwBLAEsAUABQAFAAHgAeAB4AHgAeAB4AGwAeACsAKwArACsAKwAEAAQABAAEAAQAUABQAFAAUABQAFAAUABQACsAUABQAFAAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwArACsAUAAEAAQABAAEAAQABAAEACsABAAEAAQAKwAEAAQABAAEACsAKwArACsAKwArACsABAAEACsAUABQAFAAKwArACsAKwArAFAAUAAEAAQAKwArAEsASwBLAEsASwBLAEsASwBLAEsAKwArACsAKwArACsAKwAOAFAAUABQAFAAUABQAFAAHgBQAAQABAAEAA4AUABQAFAAUABQAFAAUABQACsAUABQAFAAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArAFAAUABQAFAAUABQAFAAUABQAFAAKwBQAFAAUABQAFAAKwArAAQAUAAEAAQABAAEAAQABAAEACsABAAEAAQAKwAEAAQABAAEACsAKwArACsAKwArACsABAAEACsAKwArACsAKwArACsAUAArAFAAUAAEAAQAKwArAEsASwBLAEsASwBLAEsASwBLAEsAKwBQAFAAKwArACsAKwArACsAKwArACsAKwArACsAKwAEAAQABAAEAFAAUABQAFAAUABQAFAAUABQACsAUABQAFAAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAABAAEAFAABAAEAAQABAAEAAQABAArAAQABAAEACsABAAEAAQABABQAB4AKwArACsAKwBQAFAAUAAEAFAAUABQAFAAUABQAFAAUABQAFAABAAEACsAKwBLAEsASwBLAEsASwBLAEsASwBLAFAAUABQAFAAUABQAFAAUABQABoAUABQAFAAUABQAFAAKwAEAAQABAArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArAFAAUABQAFAAUABQAFAAUABQACsAUAArACsAUABQAFAAUABQAFAAUAArACsAKwAEACsAKwArACsABAAEAAQABAAEAAQAKwAEACsABAAEAAQABAAEAAQABAAEACsAKwArACsAKwArAEsASwBLAEsASwBLAEsASwBLAEsAKwArAAQABAAeACsAKwArACsAKwArACsAKwArACsAKwArAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXAAqAFwAXAAqACoAKgAqACoAKgAqACsAKwArACsAGwBcAFwAXABcAFwAXABcACoAKgAqACoAKgAqACoAKgAeAEsASwBLAEsASwBLAEsASwBLAEsADQANACsAKwArACsAKwBcAFwAKwBcACsAXABcAFwAXABcACsAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcACsAXAArAFwAXABcAFwAXABcAFwAXABcAFwAKgBcAFwAKgAqACoAKgAqACoAKgAqACoAXAArACsAXABcAFwAXABcACsAXAArACoAKgAqACoAKgAqACsAKwBLAEsASwBLAEsASwBLAEsASwBLACsAKwBcAFwAXABcAFAADgAOAA4ADgAeAA4ADgAJAA4ADgANAAkAEwATABMAEwATAAkAHgATAB4AHgAeAAQABAAeAB4AHgAeAB4AHgBLAEsASwBLAEsASwBLAEsASwBLAFAAUABQAFAAUABQAFAAUABQAFAADQAEAB4ABAAeAAQAFgARABYAEQAEAAQAUABQAFAAUABQAFAAUABQACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwArACsAKwAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQADQAEAAQABAAEAAQADQAEAAQAUABQAFAAUABQAAQABAAEAAQABAAEAAQABAAEAAQABAArAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAArAA0ADQAeAB4AHgAeAB4AHgAEAB4AHgAeAB4AHgAeACsAHgAeAA4ADgANAA4AHgAeAB4AHgAeAAkACQArACsAKwArACsAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcACoAKgAqACoAKgAqACoAKgAqACoAKgAqACoAKgAqACoAKgAqACoAKgBcAEsASwBLAEsASwBLAEsASwBLAEsADQANAB4AHgAeAB4AXABcAFwAXABcAFwAKgAqACoAKgBcAFwAXABcACoAKgAqAFwAKgAqACoAXABcACoAKgAqACoAKgAqACoAXABcAFwAKgAqACoAKgBcAFwAXABcAFwAXABcAFwAXABcAFwAXABcACoAKgAqACoAKgAqACoAKgAqACoAKgAqAFwAKgBLAEsASwBLAEsASwBLAEsASwBLACoAKgAqACoAKgAqAFAAUABQAFAAUABQACsAUAArACsAKwArACsAUAArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAHgBQAFAAUABQAFgAWABYAFgAWABYAFgAWABYAFgAWABYAFgAWABYAFgAWABYAFgAWABYAFgAWABYAFgAWABYAFgAWABYAFgAWABZAFkAWQBZAFkAWQBZAFkAWQBZAFkAWQBZAFkAWQBZAFkAWQBZAFkAWQBZAFkAWQBZAFkAWQBZAFkAWQBZAFkAWgBaAFoAWgBaAFoAWgBaAFoAWgBaAFoAWgBaAFoAWgBaAFoAWgBaAFoAWgBaAFoAWgBaAFoAWgBaAFoAWgBaAFAAUABQAFAAUABQAFAAUABQACsAUABQAFAAUAArACsAUABQAFAAUABQAFAAUAArAFAAKwBQAFAAUABQACsAKwBQAFAAUABQAFAAUABQAFAAUAArAFAAUABQAFAAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArAFAAUABQAFAAKwArAFAAUABQAFAAUABQAFAAKwBQACsAUABQAFAAUAArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwBQAFAAUABQACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsABAAEAAQAHgANAB4AHgAeAB4AHgAeAB4AUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAHgAeAB4AHgAeAB4AHgAeAB4AHgArACsAKwArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwBQAFAAUABQAFAAUAArACsADQBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAHgAeAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAANAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAWABEAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAA0ADQANAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwBQAFAAUABQAAQABAAEACsAKwArACsAKwArACsAKwArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAANAA0AKwArACsAKwArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAABAAEACsAKwArACsAKwArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwBQAFAAUAArAAQABAArACsAKwArACsAKwArACsAKwArACsAKwBcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAKgAqACoAKgAqACoAKgAqACoAKgAqACoAKgAqACoAKgAqACoAKgAqAA0ADQAVAFwADQAeAA0AGwBcACoAKwArAEsASwBLAEsASwBLAEsASwBLAEsAKwArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAKwAeAB4AEwATAA0ADQAOAB4AEwATAB4ABAAEAAQACQArAEsASwBLAEsASwBLAEsASwBLAEsAKwArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAKwArAFAAUABQAFAAUAAEAAQAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAAQAUAArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwAEAAQABAAEAAQABAAEAAQABAAEAAQABAArACsAKwArAAQABAAEAAQABAAEAAQABAAEAAQABAAEACsAKwArACsAHgArACsAKwATABMASwBLAEsASwBLAEsASwBLAEsASwBcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXAArACsAXABcAFwAXABcACsAKwArACsAKwArACsAKwArACsAKwBcAFwAXABcAFwAXABcAFwAXABcAFwAXAArACsAKwArAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcACsAKwArACsAKwArAEsASwBLAEsASwBLAEsASwBLAEsAXAArACsAKwAqACoAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAAQABAAEAAQABAArACsAHgAeAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcACoAKgAqACoAKgAqACoAKgAqACoAKwAqACoAKgAqACoAKgAqACoAKgAqACoAKgAqACoAKgAqACoAKgAqACoAKgAqACoAKgAqACoAKgAqACoAKwArAAQASwBLAEsASwBLAEsASwBLAEsASwArACsAKwArACsAKwBLAEsASwBLAEsASwBLAEsASwBLACsAKwArACsAKwArACoAKgAqACoAKgAqACoAXAAqACoAKgAqACoAKgArACsABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsABAAEAAQABAAEAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAAQABAAEAAQABABQAFAAUABQAFAAUABQACsAKwArACsASwBLAEsASwBLAEsASwBLAEsASwANAA0AHgANAA0ADQANAB4AHgAeAB4AHgAeAB4AHgAeAB4ABAAEAAQABAAEAAQABAAEAAQAHgAeAB4AHgAeAB4AHgAeAB4AKwArACsABAAEAAQAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAABAAEAAQABAAEAAQABAAEAAQABAAEAAQABABQAFAASwBLAEsASwBLAEsASwBLAEsASwBQAFAAUABQAFAAUABQAFAABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEACsAKwArACsAKwArACsAKwAeAB4AHgAeAFAAUABQAFAABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEACsAKwArAA0ADQANAA0ADQBLAEsASwBLAEsASwBLAEsASwBLACsAKwArAFAAUABQAEsASwBLAEsASwBLAEsASwBLAEsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAA0ADQBQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwBQAFAAUAAeAB4AHgAeAB4AHgAeAB4AKwArACsAKwArACsAKwArAAQABAAEAB4ABAAEAAQABAAEAAQABAAEAAQABAAEAAQABABQAFAAUABQAAQAUABQAFAAUABQAFAABABQAFAABAAEAAQAUAArACsAKwArACsABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEACsABAAEAAQABAAEAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AKwArAFAAUABQAFAAUABQACsAKwBQAFAAUABQAFAAUABQAFAAKwBQACsAUAArAFAAKwAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeACsAKwAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgArAB4AHgAeAB4AHgAeAB4AHgBQAB4AHgAeAFAAUABQACsAHgAeAB4AHgAeAB4AHgAeAB4AHgBQAFAAUABQACsAKwAeAB4AHgAeAB4AHgArAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AKwArAFAAUABQACsAHgAeAB4AHgAeAB4AHgAOAB4AKwANAA0ADQANAA0ADQANAAkADQANAA0ACAAEAAsABAAEAA0ACQANAA0ADAAdAB0AHgAXABcAFgAXABcAFwAWABcAHQAdAB4AHgAUABQAFAANAAEAAQAEAAQABAAEAAQACQAaABoAGgAaABoAGgAaABoAHgAXABcAHQAVABUAHgAeAB4AHgAeAB4AGAAWABEAFQAVABUAHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4ADQAeAA0ADQANAA0AHgANAA0ADQAHAB4AHgAeAB4AKwAEAAQABAAEAAQABAAEAAQABAAEAFAAUAArACsATwBQAFAAUABQAFAAHgAeAB4AFgARAE8AUABPAE8ATwBPAFAAUABQAFAAUAAeAB4AHgAWABEAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwArABsAGwAbABsAGwAbABsAGgAbABsAGwAbABsAGwAbABsAGwAbABsAGwAbABsAGgAbABsAGwAbABoAGwAbABoAGwAbABsAGwAbABsAGwAbABsAGwAbABsAGwAbABsAGwAbAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQAHgAeAFAAGgAeAB0AHgBQAB4AGgAeAB4AHgAeAB4AHgAeAB4AHgBPAB4AUAAbAB4AHgBQAFAAUABQAFAAHgAeAB4AHQAdAB4AUAAeAFAAHgBQAB4AUABPAFAAUAAeAB4AHgAeAB4AHgAeAFAAUABQAFAAUAAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAFAAHgBQAFAAUABQAE8ATwBQAFAAUABQAFAATwBQAFAATwBQAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwBPAFAAUABQAFAATwBPAE8ATwBPAE8ATwBPAE8ATwBQAFAAUABQAFAAUABQAFAAUAAeAB4AUABQAFAAUABPAB4AHgArACsAKwArAB0AHQAdAB0AHQAdAB0AHQAdAB0AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB0AHgAdAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAdAB4AHQAdAB4AHgAeAB0AHQAeAB4AHQAeAB4AHgAdAB4AHQAbABsAHgAdAB4AHgAeAB4AHQAeAB4AHQAdAB0AHQAeAB4AHQAeAB0AHgAdAB0AHQAdAB0AHQAeAB0AHgAeAB4AHgAeAB0AHQAdAB0AHgAeAB4AHgAdAB0AHgAeAB4AHgAeAB4AHgAeAB4AHgAdAB4AHgAeAB0AHgAeAB4AHgAeAB0AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAdAB0AHgAeAB0AHQAdAB0AHgAeAB0AHQAeAB4AHQAdAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB0AHQAeAB4AHQAdAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHQAeAB4AHgAdAB4AHgAeAB4AHgAeAB4AHQAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB0AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AFAAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeABYAEQAWABEAHgAeAB4AHgAeAB4AHQAeAB4AHgAeAB4AHgAeACUAJQAeAB4AHgAeAB4AHgAeAB4AHgAWABEAHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AJQAlACUAJQAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwBPAFAAHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHgAeAB4AHgAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAeAB4AHQAdAB0AHQAeAB4AHgAeAB4AHgAeAB4AHgAeAB0AHQAeAB0AHQAdAB0AHQAdAB0AHgAeAB4AHgAeAB4AHgAeAB0AHQAeAB4AHQAdAB4AHgAeAB4AHQAdAB4AHgAeAB4AHQAdAB0AHgAeAB0AHgAeAB0AHQAdAB0AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAdAB0AHQAdAB4AHgAeAB4AHgAeAB4AHgAeAB0AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAlACUAJQAlAB4AHQAdAB4AHgAdAB4AHgAeAB4AHQAdAB4AHgAeAB4AJQAlAB0AHQAlAB4AJQAlACUAIAAlACUAHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAlACUAJQAeAB4AHgAeAB0AHgAdAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAdAB0AHgAdAB0AHQAeAB0AJQAdAB0AHgAdAB0AHgAdAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeACUAHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHQAdAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAlACUAJQAlACUAJQAlACUAJQAlACUAJQAdAB0AHQAdACUAHgAlACUAJQAdACUAJQAdAB0AHQAlACUAHQAdACUAHQAdACUAJQAlAB4AHQAeAB4AHgAeAB0AHQAlAB0AHQAdAB0AHQAdACUAJQAlACUAJQAdACUAJQAgACUAHQAdACUAJQAlACUAJQAlACUAJQAeAB4AHgAlACUAIAAgACAAIAAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB0AHgAeAB4AFwAXABcAFwAXABcAHgATABMAJQAeAB4AHgAWABEAFgARABYAEQAWABEAFgARABYAEQAWABEATwBPAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeABYAEQAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAWABEAFgARABYAEQAWABEAFgARAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AFgARABYAEQAWABEAFgARABYAEQAWABEAFgARABYAEQAWABEAFgARABYAEQAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAWABEAFgARAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AFgARAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAdAB0AHQAdAB0AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgArACsAHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AKwAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AUABQAFAAUAAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAEAAQABAAeAB4AKwArACsAKwArABMADQANAA0AUAATAA0AUABQAFAAUABQAFAAUABQACsAKwArACsAKwArACsAUAANACsAKwArACsAKwArACsAKwArACsAKwArACsAKwAEAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAKwArACsAKwBQAFAAUABQAFAAUABQACsAUABQAFAAUABQAFAAUAArAFAAUABQAFAAUABQAFAAKwBQAFAAUABQAFAAUABQACsAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXAA0ADQANAA0ADQANAA0ADQAeAA0AFgANAB4AHgAXABcAHgAeABcAFwAWABEAFgARABYAEQAWABEADQANAA0ADQATAFAADQANAB4ADQANAB4AHgAeAB4AHgAMAAwADQANAA0AHgANAA0AFgANAA0ADQANAA0ADQANAA0AHgANAB4ADQANAB4AHgAeACsAKwArACsAKwArACsAKwArACsAKwArACsAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACsAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAKwArACsAKwArACsAKwArACsAKwArACsAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwAlACUAJQAlACUAJQAlACUAJQAlACUAJQArACsAKwArAA0AEQARACUAJQBHAFcAVwAWABEAFgARABYAEQAWABEAFgARACUAJQAWABEAFgARABYAEQAWABEAFQAWABEAEQAlAFcAVwBXAFcAVwBXAFcAVwBXAAQABAAEAAQABAAEACUAVwBXAFcAVwA2ACUAJQBXAFcAVwBHAEcAJQAlACUAKwBRAFcAUQBXAFEAVwBRAFcAUQBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFEAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBRAFcAUQBXAFEAVwBXAFcAVwBXAFcAUQBXAFcAVwBXAFcAVwBRAFEAKwArAAQABAAVABUARwBHAFcAFQBRAFcAUQBXAFEAVwBRAFcAUQBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFEAVwBRAFcAUQBXAFcAVwBXAFcAVwBRAFcAVwBXAFcAVwBXAFEAUQBXAFcAVwBXABUAUQBHAEcAVwArACsAKwArACsAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAKwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAKwAlACUAVwBXAFcAVwAlACUAJQAlACUAJQAlACUAJQAlACsAKwArACsAKwArACsAKwArACsAKwArAFEAUQBRAFEAUQBRAFEAUQBRAFEAUQBRAFEAUQBRAFEAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQArAFcAVwBXAFcAVwBXAFcAVwBXAFcAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQBPAE8ATwBPAE8ATwBPAE8AJQBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXACUAJQAlAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAEcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAKwArACsAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQArACsAKwArACsAKwArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAADQATAA0AUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABLAEsASwBLAEsASwBLAEsASwBLAFAAUAArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAFAABAAEAAQABAAeAAQABAAEAAQABAAEAAQABAAEAAQAHgBQAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AUABQAAQABABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAAQABAAeAA0ADQANAA0ADQArACsAKwArACsAKwArACsAHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAFAAUABQAFAAUABQAFAAUABQAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AUAAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgBQAB4AHgAeAB4AHgAeAFAAHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgArACsAHgAeAB4AHgAeAB4AHgAeAB4AKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwAeAB4AUABQAFAAUABQAFAAUABQAFAAUABQAAQAUABQAFAABABQAFAAUABQAAQAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAAQABAAEAAQABAAeAB4AHgAeAAQAKwArACsAUABQAFAAUABQAFAAHgAeABoAHgArACsAKwArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAADgAOABMAEwArACsAKwArACsAKwArACsABAAEAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAAQABAAEAAQABAAEACsAKwArACsAKwArACsAKwANAA0ASwBLAEsASwBLAEsASwBLAEsASwArACsAKwArACsAKwAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABABQAFAAUABQAFAAUAAeAB4AHgBQAA4AUABQAAQAUABQAFAAUABQAFAABAAEAAQABAAEAAQABAAEAA0ADQBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQAKwArACsAKwArACsAKwArACsAKwArAB4AWABYAFgAWABYAFgAWABYAFgAWABYAFgAWABYAFgAWABYAFgAWABYAFgAWABYAFgAWABYAFgAWABYACsAKwArAAQAHgAeAB4AHgAeAB4ADQANAA0AHgAeAB4AHgArAFAASwBLAEsASwBLAEsASwBLAEsASwArACsAKwArAB4AHgBcAFwAXABcAFwAKgBcAFwAXABcAFwAXABcAFwAXABcAEsASwBLAEsASwBLAEsASwBLAEsAXABcAFwAXABcACsAUABQAFAAUABQAFAAUABQAFAABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEACsAKwArACsAKwArACsAKwArAFAAUABQAAQAUABQAFAAUABQAFAAUABQAAQABAArACsASwBLAEsASwBLAEsASwBLAEsASwArACsAHgANAA0ADQBcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAKgAqACoAXAAqACoAKgBcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXAAqAFwAKgAqACoAXABcACoAKgBcAFwAXABcAFwAKgAqAFwAKgBcACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArAFwAXABcACoAKgBQAFAAUABQAFAAUABQAFAAUABQAFAABAAEAAQABAAEAA0ADQBQAFAAUAAEAAQAKwArACsAKwArACsAKwArACsAKwBQAFAAUABQAFAAUAArACsAUABQAFAAUABQAFAAKwArAFAAUABQAFAAUABQACsAKwArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAKwBQAFAAUABQAFAAUABQACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAHgAeACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAAEAAQABAAEAAQADQAEAAQAKwArAEsASwBLAEsASwBLAEsASwBLAEsAKwArACsAKwArACsAVABVAFUAVQBVAFUAVQBVAFUAVQBVAFUAVQBVAFUAVQBVAFUAVQBVAFUAVQBVAFUAVQBVAFUAVQBUAFUAVQBVAFUAVQBVAFUAVQBVAFUAVQBVAFUAVQBVAFUAVQBVAFUAVQBVAFUAVQBVAFUAVQBVACsAKwArACsAKwArACsAKwArACsAKwArAFkAWQBZAFkAWQBZAFkAWQBZAFkAWQBZAFkAWQBZAFkAWQBZAFkAKwArACsAKwBaAFoAWgBaAFoAWgBaAFoAWgBaAFoAWgBaAFoAWgBaAFoAWgBaAFoAWgBaAFoAWgBaAFoAWgBaAFoAKwArACsAKwAGAAYABgAGAAYABgAGAAYABgAGAAYABgAGAAYABgAGAAYABgAGAAYABgAGAAYABgAGAAYABgAGAAYABgAGAAYAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXACUAJQBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAJQAlACUAJQAlACUAUABQAFAAUABQAFAAUAArACsAKwArACsAKwArACsAKwArACsAKwBQAFAAUABQAFAAKwArACsAKwArAFYABABWAFYAVgBWAFYAVgBWAFYAVgBWAB4AVgBWAFYAVgBWAFYAVgBWAFYAVgBWAFYAVgArAFYAVgBWAFYAVgArAFYAKwBWAFYAKwBWAFYAKwBWAFYAVgBWAFYAVgBWAFYAVgBWAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAEQAWAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUAAaAB4AKwArAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQAGAARABEAGAAYABMAEwAWABEAFAArACsAKwArACsAKwAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEACUAJQAlACUAJQAWABEAFgARABYAEQAWABEAFgARABYAEQAlACUAFgARACUAJQAlACUAJQAlACUAEQAlABEAKwAVABUAEwATACUAFgARABYAEQAWABEAJQAlACUAJQAlACUAJQAlACsAJQAbABoAJQArACsAKwArAFAAUABQAFAAUAArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwArAAcAKwATACUAJQAbABoAJQAlABYAEQAlACUAEQAlABEAJQBXAFcAVwBXAFcAVwBXAFcAVwBXABUAFQAlACUAJQATACUAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXABYAJQARACUAJQAlAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwAWACUAEQAlABYAEQARABYAEQARABUAVwBRAFEAUQBRAFEAUQBRAFEAUQBRAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAEcARwArACsAVwBXAFcAVwBXAFcAKwArAFcAVwBXAFcAVwBXACsAKwBXAFcAVwBXAFcAVwArACsAVwBXAFcAKwArACsAGgAbACUAJQAlABsAGwArAB4AHgAeAB4AHgAeAB4AKwArACsAKwArACsAKwArACsAKwAEAAQABAAQAB0AKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwBQAFAAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsADQANAA0AKwArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwArAB4AHgAeAB4AHgAeAB4AHgAeAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgBQAFAAHgAeAB4AKwAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAAQAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwAEAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAABAAEAAQABAAEACsAKwArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArAA0AUABQAFAAUAArACsAKwArAFAAUABQAFAAUABQAFAAUAANAFAAUABQAFAAUAArACsAKwArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwArACsAKwArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwArACsAKwArACsAKwArACsAKwAeACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAUABQAFAAUABQAFAAKwArAFAAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArAFAAUAArACsAKwBQACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwANAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAeAB4AUABQAFAAUABQAFAAUAArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArAFAAUAArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwArAA0AUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwArACsAKwAeAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwArACsAUABQAFAAUABQAAQABAAEACsABAAEACsAKwArACsAKwAEAAQABAAEAFAAUABQAFAAKwBQAFAAUAArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwArAAQABAAEACsAKwArACsABABQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAKwArAA0ADQANAA0ADQANAA0ADQAeACsAKwArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAeAFAAUABQAFAAUABQAFAAUAAeAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAAQABAArACsAKwArAFAAUABQAFAAUAANAA0ADQANAA0ADQAUACsAKwArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwArACsADQANAA0ADQANAA0ADQBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAKwArAB4AHgAeAB4AKwArACsAKwArACsAKwArACsAKwArACsAUABQAFAAUABQAFAAUAArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwArACsAKwArACsAKwArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAKwArAFAAUABQAFAAUABQAAQABAAEAAQAKwArACsAKwArACsAKwArAEsASwBLAEsASwBLAEsASwBLAEsAKwArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUAArAAQABAANACsAKwBQAFAAKwArACsAKwArACsAKwArACsAKwArACsAKwArAFAAUABQAFAAUABQAAQABAAEAAQABAAEAAQABAAEAAQABABQAFAAUABQAB4AHgAeAB4AHgArACsAKwArACsAKwAEAAQABAAEAAQABAAEAA0ADQAeAB4AHgAeAB4AKwArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAEsASwBLAEsASwBLAEsASwBLAEsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsABABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAAQABAAEAAQABAAEAAQABAAEAAQABAAeAB4AHgANAA0ADQANACsAKwArACsAKwArACsAKwArACsAKwAeACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwArACsAKwArACsAKwBLAEsASwBLAEsASwBLAEsASwBLACsAKwArACsAKwArAFAAUABQAFAAUABQAFAABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEACsASwBLAEsASwBLAEsASwBLAEsASwANAA0ADQANAFAABAAEAFAAKwArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAABAAeAA4AUAArACsAKwArACsAKwArACsAKwAEAFAAUABQAFAADQANAB4ADQAEAAQABAAEAB4ABAAEAEsASwBLAEsASwBLAEsASwBLAEsAUAAOAFAADQANAA0AKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwArACsAKwArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAAEAAQABAAEAAQABAAEAAQABAANAA0AHgANAA0AHgAEACsAUABQAFAAUABQAFAAUAArAFAAKwBQAFAAUABQACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwBQAFAAUABQAFAAUABQAFAAUABQAA0AKwArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAAEAAQABAAEAAQABAAEAAQAKwArACsAKwArAEsASwBLAEsASwBLAEsASwBLAEsAKwArACsAKwArACsABAAEAAQABAArAFAAUABQAFAAUABQAFAAUAArACsAUABQACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwBQAFAAUABQAFAAUABQACsAUABQACsAUABQAFAAUABQACsABAAEAFAABAAEAAQABAAEAAQABAArACsABAAEACsAKwAEAAQABAArACsAUAArACsAKwArACsAKwAEACsAKwArACsAKwBQAFAAUABQAFAABAAEACsAKwAEAAQABAAEAAQABAAEACsAKwArAAQABAAEAAQABAArACsAKwArACsAKwArACsAKwArACsABAAEAAQABAAEAAQABABQAFAAUABQAA0ADQANAA0AHgBLAEsASwBLAEsASwBLAEsASwBLAA0ADQArAB4ABABQAFAAUAArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwAEAAQABAAEAFAAUAAeAFAAKwArACsAKwArACsAKwArAEsASwBLAEsASwBLAEsASwBLAEsAKwArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAABAAEAAQABAAEAAQABAArACsABAAEAAQABAAEAAQABAAEAAQADgANAA0AEwATAB4AHgAeAA0ADQANAA0ADQANAA0ADQANAA0ADQANAA0ADQANAFAAUABQAFAABAAEACsAKwAEAA0ADQAeAFAAKwArACsAKwArACsAKwArACsAKwArAEsASwBLAEsASwBLAEsASwBLAEsAKwArACsAKwArACsADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAFAAKwArACsAKwArACsAKwBLAEsASwBLAEsASwBLAEsASwBLACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAXABcAFwAKwArACoAKgAqACoAKgAqACoAKgAqACoAKgAqACoAKgAqACsAKwArACsASwBLAEsASwBLAEsASwBLAEsASwBcAFwADQANAA0AKgBQAFAAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAeACsAKwArACsASwBLAEsASwBLAEsASwBLAEsASwBQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAKwArACsAKwArACsAKwBQAFAAUABQAFAAUABQAFAAKwArAFAAKwArAFAAUABQAFAAUABQAFAAUAArAFAAUAArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAABAAEAAQABAAEAAQAKwAEAAQAKwArAAQABAAEAAQAUAAEAFAABAAEAA0ADQANACsAKwArACsAKwArACsAKwArAEsASwBLAEsASwBLAEsASwBLAEsAKwArACsAKwArACsAUABQAFAAUABQAFAAUABQACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAABAAEAAQABAAEAAQABAArACsABAAEAAQABAAEAAQABABQAA4AUAAEACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArAFAABAAEAAQABAAEAAQABAAEAAQABABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAAEAAQABAAEAFAABAAEAAQABAAOAB4ADQANAA0ADQAOAB4ABAArACsAKwArACsAKwArACsAUAAEAAQABAAEAAQABAAEAAQABAAEAAQAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAA0ADQANAFAADgAOAA4ADQANACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwBQAFAAUABQAFAAUABQAFAAUAArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAABAAEAAQABAAEAAQABAAEACsABAAEAAQABAAEAAQABAAEAFAADQANAA0ADQANACsAKwArACsAKwArACsAKwArACsASwBLAEsASwBLAEsASwBLAEsASwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwAOABMAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwArAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAArAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAArACsAKwArACsAKwArACsAKwBQAFAAUABQAFAAUABQACsAUABQACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAAEAAQABAArACsAKwAEACsABAAEACsABAAEAAQABAAEAAQABABQAAQAKwArACsAKwArACsAKwArAEsASwBLAEsASwBLAEsASwBLAEsAKwArACsAKwArACsAUABQAFAAUABQAFAAKwBQAFAAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAAEAAQAKwAEAAQAKwAEAAQABAAEAAQAUAArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAABAAEAAQABAAeAB4AKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwBQACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAB4AHgAeAB4AHgAeAB4AHgAaABoAGgAaAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgArACsAKwArACsAKwArACsAKwArACsAKwArAA0AUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsADQANAA0ADQANACsAKwArACsAKwArACsAKwArACsAKwBQAFAAUABQACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAASABIAEgAQwBDAEMAUABQAFAAUABDAFAAUABQAEgAQwBIAEMAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAASABDAEMAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwAJAAkACQAJAAkACQAJABYAEQArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABIAEMAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArAEsASwBLAEsASwBLAEsASwBLAEsAKwArACsAKwANAA0AKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwArAAQABAAEAAQABAANACsAKwArACsAKwArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAAEAAQABAAEAA0ADQANAB4AHgAeAB4AHgAeAFAAUABQAFAADQAeACsAKwArACsAKwArACsAKwArACsASwBLAEsASwBLAEsASwBLAEsASwArAFAAUABQAFAAUABQAFAAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAANAA0AHgAeACsAKwArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAKwArACsAKwAEAFAABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQAKwArACsAKwArACsAKwAEAAQABAAEAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAARwBHABUARwAJACsAKwArACsAKwArACsAKwArACsAKwAEAAQAKwArACsAKwArACsAKwArACsAKwArACsAKwArAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXACsAKwArACsAKwArACsAKwBXAFcAVwBXAFcAVwBXAFcAVwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAUQBRAFEAKwArACsAKwArACsAKwArACsAKwArACsAKwBRAFEAUQBRACsAKwArACsAKwArACsAKwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUAArACsAHgAEAAQADQAEAAQABAAEACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgArACsAKwArACsAKwArACsAKwArAB4AHgAeAB4AHgAeAB4AKwArAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAAQABAAEAAQABAAeAB4AHgAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAB4AHgAEAAQABAAEAAQABAAEAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4ABAAEAAQABAAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4ABAAEAAQAHgArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwArACsAKwArACsAKwArACsAKwArAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgArACsAKwArACsAKwArACsAKwAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgArAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AKwBQAFAAKwArAFAAKwArAFAAUAArACsAUABQAFAAUAArAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeACsAUAArAFAAUABQAFAAUABQAFAAKwAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AKwBQAFAAUABQACsAKwBQAFAAUABQAFAAUABQAFAAKwBQAFAAUABQAFAAUABQACsAHgAeAFAAUABQAFAAUAArAFAAKwArACsAUABQAFAAUABQAFAAUAArAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAHgBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgBQAFAAUABQAFAAUABQAFAAUABQAFAAHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAB4AHgAeAB4AHgAeAB4AHgAeACsAKwBLAEsASwBLAEsASwBLAEsASwBLAEsASwBLAEsASwBLAEsASwBLAEsASwBLAEsASwBLAEsASwBLAEsASwBLAEsASwBLAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAeAB4AHgAeAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAeAB4AHgAeAB4AHgAeAB4ABAAeAB4AHgAeAB4AHgAeAB4AHgAeAAQAHgAeAA0ADQANAA0AHgArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwAEAAQABAAEAAQAKwAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArAAQABAAEAAQABAAEAAQAKwAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQAKwArAAQABAAEAAQABAAEAAQAKwAEAAQAKwAEAAQABAAEAAQAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwAEAAQABAAEAAQABAAEAFAAUABQAFAAUABQAFAAKwArAEsASwBLAEsASwBLAEsASwBLAEsAKwArACsAKwBQAB4AKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUAAEAAQABAAEAEsASwBLAEsASwBLAEsASwBLAEsAKwArACsAKwArABsAUABQAFAAUABQACsAKwBQAFAAUABQAFAAUABQAFAAUAAEAAQABAAEAAQABAAEACsAKwArACsAKwArACsAKwArAB4AHgAeAB4ABAAEAAQABAAEAAQABABQACsAKwArACsASwBLAEsASwBLAEsASwBLAEsASwArACsAKwArABYAFgArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAGgBQAFAAUAAaAFAAUABQAFAAKwArACsAKwArACsAKwArACsAKwArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAeAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQACsAKwBQAFAAUABQACsAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwBQAFAAKwBQACsAKwBQACsAUABQAFAAUABQAFAAUABQAFAAUAArAFAAUABQAFAAKwBQACsAUAArACsAKwArACsAKwBQACsAKwArACsAUAArAFAAKwBQACsAUABQAFAAKwBQAFAAKwBQACsAKwBQACsAUAArAFAAKwBQACsAUAArAFAAUAArAFAAKwArAFAAUABQAFAAKwBQAFAAUABQAFAAUABQACsAUABQAFAAUAArAFAAUABQAFAAKwBQACsAUABQAFAAUABQAFAAUABQAFAAUAArAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAArACsAKwArACsAUABQAFAAKwBQAFAAUABQAFAAKwBQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwAeAB4AKwArACsAKwArACsAKwArACsAKwArACsAKwArAE8ATwBPAE8ATwBPAE8ATwBPAE8ATwBPAE8AJQAlACUAHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHgAeAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB4AHgAeACUAJQAlAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAdAB0AHQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQApACkAKQApACkAKQApACkAKQApACkAKQApACkAKQApACkAKQApACkAKQApACkAKQApACkAJQAlACUAJQAlACAAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAeAB4AJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlAB4AHgAlACUAJQAlACUAHgAlACUAJQAlACUAIAAgACAAJQAlACAAJQAlACAAIAAgACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACEAIQAhACEAIQAlACUAIAAgACUAJQAgACAAIAAgACAAIAAgACAAIAAgACAAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAJQAlACUAIAAlACUAJQAlACAAIAAgACUAIAAgACAAJQAlACUAJQAlACUAJQAgACUAIAAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAHgAlAB4AJQAeACUAJQAlACUAJQAgACUAJQAlACUAHgAlAB4AHgAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlAB4AHgAeAB4AHgAeAB4AJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAeAB4AHgAeAB4AHgAeAB4AHgAeACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACAAIAAlACUAJQAlACAAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACAAJQAlACUAJQAgACAAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAHgAeAB4AHgAeAB4AHgAeACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAeAB4AHgAeAB4AHgAlACUAJQAlACUAJQAlACAAIAAgACUAJQAlACAAIAAgACAAIAAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeABcAFwAXABUAFQAVAB4AHgAeAB4AJQAlACUAIAAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACAAIAAgACUAJQAlACUAJQAlACUAJQAlACAAJQAlACUAJQAlACUAJQAlACUAJQAlACAAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AJQAlACUAJQAlACUAJQAlACUAJQAlACUAHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AJQAlACUAJQAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeACUAJQAlACUAJQAlACUAJQAeAB4AHgAeAB4AHgAeAB4AHgAeACUAJQAlACUAJQAlAB4AHgAeAB4AHgAeAB4AHgAlACUAJQAlACUAJQAlACUAHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAgACUAJQAgACUAJQAlACUAJQAlACUAJQAgACAAIAAgACAAIAAgACAAJQAlACUAJQAlACUAIAAlACUAJQAlACUAJQAlACUAJQAgACAAIAAgACAAIAAgACAAIAAgACUAJQAgACAAIAAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAgACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACAAIAAlACAAIAAlACAAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAgACAAIAAlACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAJQAlAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AKwAeAB4AHgAeAB4AHgAeAB4AHgAeAB4AHgArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArAEsASwBLAEsASwBLAEsASwBLAEsAKwArACsAKwArACsAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAKwArAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXACUAJQBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwAlACUAJQAlACUAJQAlACUAJQAlACUAVwBXACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQBXAFcAVwBXAFcAVwBXAFcAVwBXAFcAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAJQAlACUAKwAEACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArACsAKwArAA==';

    var LETTER_NUMBER_MODIFIER = 50;
    // Non-tailorable Line Breaking Classes
    var BK = 1; //  Cause a line break (after)
    var CR$1 = 2; //  Cause a line break (after), except between CR and LF
    var LF$1 = 3; //  Cause a line break (after)
    var CM = 4; //  Prohibit a line break between the character and the preceding character
    var NL = 5; //  Cause a line break (after)
    var WJ = 7; //  Prohibit line breaks before and after
    var ZW = 8; //  Provide a break opportunity
    var GL = 9; //  Prohibit line breaks before and after
    var SP = 10; // Enable indirect line breaks
    var ZWJ$1 = 11; // Prohibit line breaks within joiner sequences
    // Break Opportunities
    var B2 = 12; //  Provide a line break opportunity before and after the character
    var BA = 13; //  Generally provide a line break opportunity after the character
    var BB = 14; //  Generally provide a line break opportunity before the character
    var HY = 15; //  Provide a line break opportunity after the character, except in numeric context
    var CB = 16; //   Provide a line break opportunity contingent on additional information
    // Characters Prohibiting Certain Breaks
    var CL = 17; //  Prohibit line breaks before
    var CP = 18; //  Prohibit line breaks before
    var EX = 19; //  Prohibit line breaks before
    var IN = 20; //  Allow only indirect line breaks between pairs
    var NS = 21; //  Allow only indirect line breaks before
    var OP = 22; //  Prohibit line breaks after
    var QU = 23; //  Act like they are both opening and closing
    // Numeric Context
    var IS = 24; //  Prevent breaks after any and before numeric
    var NU = 25; //  Form numeric expressions for line breaking purposes
    var PO = 26; //  Do not break following a numeric expression
    var PR = 27; //  Do not break in front of a numeric expression
    var SY = 28; //  Prevent a break before; and allow a break after
    // Other Characters
    var AI = 29; //  Act like AL when the resolvedEAW is N; otherwise; act as ID
    var AL = 30; //  Are alphabetic characters or symbols that are used with alphabetic characters
    var CJ = 31; //  Treat as NS or ID for strict or normal breaking.
    var EB = 32; //  Do not break from following Emoji Modifier
    var EM = 33; //  Do not break from preceding Emoji Base
    var H2 = 34; //  Form Korean syllable blocks
    var H3 = 35; //  Form Korean syllable blocks
    var HL = 36; //  Do not break around a following hyphen; otherwise act as Alphabetic
    var ID = 37; //  Break before or after; except in some numeric context
    var JL = 38; //  Form Korean syllable blocks
    var JV = 39; //  Form Korean syllable blocks
    var JT = 40; //  Form Korean syllable blocks
    var RI$1 = 41; //  Keep pairs together. For pairs; break before and after other classes
    var SA = 42; //  Provide a line break opportunity contingent on additional, language-specific context analysis
    var XX = 43; //  Have as yet unknown line breaking behavior or unassigned code positions
    var ea_OP = [0x2329, 0xff08];
    var BREAK_MANDATORY = '!';
    var BREAK_NOT_ALLOWED$1 = '×';
    var BREAK_ALLOWED$1 = '÷';
    var UnicodeTrie$1 = createTrieFromBase64$1(base64$1);
    var ALPHABETICS = [AL, HL];
    var HARD_LINE_BREAKS = [BK, CR$1, LF$1, NL];
    var SPACE$1 = [SP, ZW];
    var PREFIX_POSTFIX = [PR, PO];
    var LINE_BREAKS = HARD_LINE_BREAKS.concat(SPACE$1);
    var KOREAN_SYLLABLE_BLOCK = [JL, JV, JT, H2, H3];
    var HYPHEN = [HY, BA];
    var codePointsToCharacterClasses = function (codePoints, lineBreak) {
        if (lineBreak === void 0) { lineBreak = 'strict'; }
        var types = [];
        var indices = [];
        var categories = [];
        codePoints.forEach(function (codePoint, index) {
            var classType = UnicodeTrie$1.get(codePoint);
            if (classType > LETTER_NUMBER_MODIFIER) {
                categories.push(true);
                classType -= LETTER_NUMBER_MODIFIER;
            }
            else {
                categories.push(false);
            }
            if (['normal', 'auto', 'loose'].indexOf(lineBreak) !== -1) {
                // U+2010, – U+2013, 〜 U+301C, ゠ U+30A0
                if ([0x2010, 0x2013, 0x301c, 0x30a0].indexOf(codePoint) !== -1) {
                    indices.push(index);
                    return types.push(CB);
                }
            }
            if (classType === CM || classType === ZWJ$1) {
                // LB10 Treat any remaining combining mark or ZWJ as AL.
                if (index === 0) {
                    indices.push(index);
                    return types.push(AL);
                }
                // LB9 Do not break a combining character sequence; treat it as if it has the line breaking class of
                // the base character in all of the following rules. Treat ZWJ as if it were CM.
                var prev = types[index - 1];
                if (LINE_BREAKS.indexOf(prev) === -1) {
                    indices.push(indices[index - 1]);
                    return types.push(prev);
                }
                indices.push(index);
                return types.push(AL);
            }
            indices.push(index);
            if (classType === CJ) {
                return types.push(lineBreak === 'strict' ? NS : ID);
            }
            if (classType === SA) {
                return types.push(AL);
            }
            if (classType === AI) {
                return types.push(AL);
            }
            // For supplementary characters, a useful default is to treat characters in the range 10000..1FFFD as AL
            // and characters in the ranges 20000..2FFFD and 30000..3FFFD as ID, until the implementation can be revised
            // to take into account the actual line breaking properties for these characters.
            if (classType === XX) {
                if ((codePoint >= 0x20000 && codePoint <= 0x2fffd) || (codePoint >= 0x30000 && codePoint <= 0x3fffd)) {
                    return types.push(ID);
                }
                else {
                    return types.push(AL);
                }
            }
            types.push(classType);
        });
        return [indices, types, categories];
    };
    var isAdjacentWithSpaceIgnored = function (a, b, currentIndex, classTypes) {
        var current = classTypes[currentIndex];
        if (Array.isArray(a) ? a.indexOf(current) !== -1 : a === current) {
            var i = currentIndex;
            while (i <= classTypes.length) {
                i++;
                var next = classTypes[i];
                if (next === b) {
                    return true;
                }
                if (next !== SP) {
                    break;
                }
            }
        }
        if (current === SP) {
            var i = currentIndex;
            while (i > 0) {
                i--;
                var prev = classTypes[i];
                if (Array.isArray(a) ? a.indexOf(prev) !== -1 : a === prev) {
                    var n = currentIndex;
                    while (n <= classTypes.length) {
                        n++;
                        var next = classTypes[n];
                        if (next === b) {
                            return true;
                        }
                        if (next !== SP) {
                            break;
                        }
                    }
                }
                if (prev !== SP) {
                    break;
                }
            }
        }
        return false;
    };
    var previousNonSpaceClassType = function (currentIndex, classTypes) {
        var i = currentIndex;
        while (i >= 0) {
            var type = classTypes[i];
            if (type === SP) {
                i--;
            }
            else {
                return type;
            }
        }
        return 0;
    };
    var _lineBreakAtIndex = function (codePoints, classTypes, indicies, index, forbiddenBreaks) {
        if (indicies[index] === 0) {
            return BREAK_NOT_ALLOWED$1;
        }
        var currentIndex = index - 1;
        if (Array.isArray(forbiddenBreaks) && forbiddenBreaks[currentIndex] === true) {
            return BREAK_NOT_ALLOWED$1;
        }
        var beforeIndex = currentIndex - 1;
        var afterIndex = currentIndex + 1;
        var current = classTypes[currentIndex];
        // LB4 Always break after hard line breaks.
        // LB5 Treat CR followed by LF, as well as CR, LF, and NL as hard line breaks.
        var before = beforeIndex >= 0 ? classTypes[beforeIndex] : 0;
        var next = classTypes[afterIndex];
        if (current === CR$1 && next === LF$1) {
            return BREAK_NOT_ALLOWED$1;
        }
        if (HARD_LINE_BREAKS.indexOf(current) !== -1) {
            return BREAK_MANDATORY;
        }
        // LB6 Do not break before hard line breaks.
        if (HARD_LINE_BREAKS.indexOf(next) !== -1) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB7 Do not break before spaces or zero width space.
        if (SPACE$1.indexOf(next) !== -1) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB8 Break before any character following a zero-width space, even if one or more spaces intervene.
        if (previousNonSpaceClassType(currentIndex, classTypes) === ZW) {
            return BREAK_ALLOWED$1;
        }
        // LB8a Do not break after a zero width joiner.
        if (UnicodeTrie$1.get(codePoints[currentIndex]) === ZWJ$1) {
            return BREAK_NOT_ALLOWED$1;
        }
        // zwj emojis
        if ((current === EB || current === EM) && UnicodeTrie$1.get(codePoints[afterIndex]) === ZWJ$1) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB11 Do not break before or after Word joiner and related characters.
        if (current === WJ || next === WJ) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB12 Do not break after NBSP and related characters.
        if (current === GL) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB12a Do not break before NBSP and related characters, except after spaces and hyphens.
        if ([SP, BA, HY].indexOf(current) === -1 && next === GL) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB13 Do not break before ‘]’ or ‘!’ or ‘;’ or ‘/’, even after spaces.
        if ([CL, CP, EX, IS, SY].indexOf(next) !== -1) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB14 Do not break after ‘[’, even after spaces.
        if (previousNonSpaceClassType(currentIndex, classTypes) === OP) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB15 Do not break within ‘”[’, even with intervening spaces.
        if (isAdjacentWithSpaceIgnored(QU, OP, currentIndex, classTypes)) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB16 Do not break between closing punctuation and a nonstarter (lb=NS), even with intervening spaces.
        if (isAdjacentWithSpaceIgnored([CL, CP], NS, currentIndex, classTypes)) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB17 Do not break within ‘——’, even with intervening spaces.
        if (isAdjacentWithSpaceIgnored(B2, B2, currentIndex, classTypes)) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB18 Break after spaces.
        if (current === SP) {
            return BREAK_ALLOWED$1;
        }
        // LB19 Do not break before or after quotation marks, such as ‘ ” ’.
        if (current === QU || next === QU) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB20 Break before and after unresolved CB.
        if (next === CB || current === CB) {
            return BREAK_ALLOWED$1;
        }
        // LB21 Do not break before hyphen-minus, other hyphens, fixed-width spaces, small kana, and other non-starters, or after acute accents.
        if ([BA, HY, NS].indexOf(next) !== -1 || current === BB) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB21a Don't break after Hebrew + Hyphen.
        if (before === HL && HYPHEN.indexOf(current) !== -1) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB21b Don’t break between Solidus and Hebrew letters.
        if (current === SY && next === HL) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB22 Do not break before ellipsis.
        if (next === IN) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB23 Do not break between digits and letters.
        if ((ALPHABETICS.indexOf(next) !== -1 && current === NU) || (ALPHABETICS.indexOf(current) !== -1 && next === NU)) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB23a Do not break between numeric prefixes and ideographs, or between ideographs and numeric postfixes.
        if ((current === PR && [ID, EB, EM].indexOf(next) !== -1) ||
            ([ID, EB, EM].indexOf(current) !== -1 && next === PO)) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB24 Do not break between numeric prefix/postfix and letters, or between letters and prefix/postfix.
        if ((ALPHABETICS.indexOf(current) !== -1 && PREFIX_POSTFIX.indexOf(next) !== -1) ||
            (PREFIX_POSTFIX.indexOf(current) !== -1 && ALPHABETICS.indexOf(next) !== -1)) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB25 Do not break between the following pairs of classes relevant to numbers:
        if (
        // (PR | PO) × ( OP | HY )? NU
        ([PR, PO].indexOf(current) !== -1 &&
            (next === NU || ([OP, HY].indexOf(next) !== -1 && classTypes[afterIndex + 1] === NU))) ||
            // ( OP | HY ) × NU
            ([OP, HY].indexOf(current) !== -1 && next === NU) ||
            // NU ×	(NU | SY | IS)
            (current === NU && [NU, SY, IS].indexOf(next) !== -1)) {
            return BREAK_NOT_ALLOWED$1;
        }
        // NU (NU | SY | IS)* × (NU | SY | IS | CL | CP)
        if ([NU, SY, IS, CL, CP].indexOf(next) !== -1) {
            var prevIndex = currentIndex;
            while (prevIndex >= 0) {
                var type = classTypes[prevIndex];
                if (type === NU) {
                    return BREAK_NOT_ALLOWED$1;
                }
                else if ([SY, IS].indexOf(type) !== -1) {
                    prevIndex--;
                }
                else {
                    break;
                }
            }
        }
        // NU (NU | SY | IS)* (CL | CP)? × (PO | PR))
        if ([PR, PO].indexOf(next) !== -1) {
            var prevIndex = [CL, CP].indexOf(current) !== -1 ? beforeIndex : currentIndex;
            while (prevIndex >= 0) {
                var type = classTypes[prevIndex];
                if (type === NU) {
                    return BREAK_NOT_ALLOWED$1;
                }
                else if ([SY, IS].indexOf(type) !== -1) {
                    prevIndex--;
                }
                else {
                    break;
                }
            }
        }
        // LB26 Do not break a Korean syllable.
        if ((JL === current && [JL, JV, H2, H3].indexOf(next) !== -1) ||
            ([JV, H2].indexOf(current) !== -1 && [JV, JT].indexOf(next) !== -1) ||
            ([JT, H3].indexOf(current) !== -1 && next === JT)) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB27 Treat a Korean Syllable Block the same as ID.
        if ((KOREAN_SYLLABLE_BLOCK.indexOf(current) !== -1 && [IN, PO].indexOf(next) !== -1) ||
            (KOREAN_SYLLABLE_BLOCK.indexOf(next) !== -1 && current === PR)) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB28 Do not break between alphabetics (“at”).
        if (ALPHABETICS.indexOf(current) !== -1 && ALPHABETICS.indexOf(next) !== -1) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB29 Do not break between numeric punctuation and alphabetics (“e.g.”).
        if (current === IS && ALPHABETICS.indexOf(next) !== -1) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB30 Do not break between letters, numbers, or ordinary symbols and opening or closing parentheses.
        if ((ALPHABETICS.concat(NU).indexOf(current) !== -1 &&
            next === OP &&
            ea_OP.indexOf(codePoints[afterIndex]) === -1) ||
            (ALPHABETICS.concat(NU).indexOf(next) !== -1 && current === CP)) {
            return BREAK_NOT_ALLOWED$1;
        }
        // LB30a Break between two regional indicator symbols if and only if there are an even number of regional
        // indicators preceding the position of the break.
        if (current === RI$1 && next === RI$1) {
            var i = indicies[currentIndex];
            var count = 1;
            while (i > 0) {
                i--;
                if (classTypes[i] === RI$1) {
                    count++;
                }
                else {
                    break;
                }
            }
            if (count % 2 !== 0) {
                return BREAK_NOT_ALLOWED$1;
            }
        }
        // LB30b Do not break between an emoji base and an emoji modifier.
        if (current === EB && next === EM) {
            return BREAK_NOT_ALLOWED$1;
        }
        return BREAK_ALLOWED$1;
    };
    var cssFormattedClasses = function (codePoints, options) {
        if (!options) {
            options = { lineBreak: 'normal', wordBreak: 'normal' };
        }
        var _a = codePointsToCharacterClasses(codePoints, options.lineBreak), indicies = _a[0], classTypes = _a[1], isLetterNumber = _a[2];
        if (options.wordBreak === 'break-all' || options.wordBreak === 'break-word') {
            classTypes = classTypes.map(function (type) { return ([NU, AL, SA].indexOf(type) !== -1 ? ID : type); });
        }
        var forbiddenBreakpoints = options.wordBreak === 'keep-all'
            ? isLetterNumber.map(function (letterNumber, i) {
                return letterNumber && codePoints[i] >= 0x4e00 && codePoints[i] <= 0x9fff;
            })
            : undefined;
        return [indicies, classTypes, forbiddenBreakpoints];
    };
    var Break = /** @class */ (function () {
        function Break(codePoints, lineBreak, start, end) {
            this.codePoints = codePoints;
            this.required = lineBreak === BREAK_MANDATORY;
            this.start = start;
            this.end = end;
        }
        Break.prototype.slice = function () {
            return fromCodePoint$1.apply(void 0, this.codePoints.slice(this.start, this.end));
        };
        return Break;
    }());
    var LineBreaker = function (str, options) {
        var codePoints = toCodePoints$1(str);
        var _a = cssFormattedClasses(codePoints, options), indicies = _a[0], classTypes = _a[1], forbiddenBreakpoints = _a[2];
        var length = codePoints.length;
        var lastEnd = 0;
        var nextIndex = 0;
        return {
            next: function () {
                if (nextIndex >= length) {
                    return { done: true, value: null };
                }
                var lineBreak = BREAK_NOT_ALLOWED$1;
                while (nextIndex < length &&
                    (lineBreak = _lineBreakAtIndex(codePoints, classTypes, indicies, ++nextIndex, forbiddenBreakpoints)) ===
                        BREAK_NOT_ALLOWED$1) { }
                if (lineBreak !== BREAK_NOT_ALLOWED$1 || nextIndex === length) {
                    var value = new Break(codePoints, lineBreak, lastEnd, nextIndex);
                    lastEnd = nextIndex;
                    return { value: value, done: false };
                }
                return { done: true, value: null };
            },
        };
    };

    // https://www.w3.org/TR/css-syntax-3
    var FLAG_UNRESTRICTED = 1 << 0;
    var FLAG_ID = 1 << 1;
    var FLAG_INTEGER = 1 << 2;
    var FLAG_NUMBER = 1 << 3;
    var LINE_FEED = 0x000a;
    var SOLIDUS = 0x002f;
    var REVERSE_SOLIDUS = 0x005c;
    var CHARACTER_TABULATION = 0x0009;
    var SPACE = 0x0020;
    var QUOTATION_MARK = 0x0022;
    var EQUALS_SIGN = 0x003d;
    var NUMBER_SIGN = 0x0023;
    var DOLLAR_SIGN = 0x0024;
    var PERCENTAGE_SIGN = 0x0025;
    var APOSTROPHE = 0x0027;
    var LEFT_PARENTHESIS = 0x0028;
    var RIGHT_PARENTHESIS = 0x0029;
    var LOW_LINE = 0x005f;
    var HYPHEN_MINUS = 0x002d;
    var EXCLAMATION_MARK = 0x0021;
    var LESS_THAN_SIGN = 0x003c;
    var GREATER_THAN_SIGN = 0x003e;
    var COMMERCIAL_AT = 0x0040;
    var LEFT_SQUARE_BRACKET = 0x005b;
    var RIGHT_SQUARE_BRACKET = 0x005d;
    var CIRCUMFLEX_ACCENT = 0x003d;
    var LEFT_CURLY_BRACKET = 0x007b;
    var QUESTION_MARK = 0x003f;
    var RIGHT_CURLY_BRACKET = 0x007d;
    var VERTICAL_LINE = 0x007c;
    var TILDE = 0x007e;
    var CONTROL = 0x0080;
    var REPLACEMENT_CHARACTER = 0xfffd;
    var ASTERISK = 0x002a;
    var PLUS_SIGN = 0x002b;
    var COMMA = 0x002c;
    var COLON = 0x003a;
    var SEMICOLON = 0x003b;
    var FULL_STOP = 0x002e;
    var NULL = 0x0000;
    var BACKSPACE = 0x0008;
    var LINE_TABULATION = 0x000b;
    var SHIFT_OUT = 0x000e;
    var INFORMATION_SEPARATOR_ONE = 0x001f;
    var DELETE = 0x007f;
    var EOF = -1;
    var ZERO = 0x0030;
    var a = 0x0061;
    var e = 0x0065;
    var f = 0x0066;
    var u = 0x0075;
    var z = 0x007a;
    var A = 0x0041;
    var E = 0x0045;
    var F = 0x0046;
    var U = 0x0055;
    var Z = 0x005a;
    var isDigit = function (codePoint) { return codePoint >= ZERO && codePoint <= 0x0039; };
    var isSurrogateCodePoint = function (codePoint) { return codePoint >= 0xd800 && codePoint <= 0xdfff; };
    var isHex = function (codePoint) {
        return isDigit(codePoint) || (codePoint >= A && codePoint <= F) || (codePoint >= a && codePoint <= f);
    };
    var isLowerCaseLetter = function (codePoint) { return codePoint >= a && codePoint <= z; };
    var isUpperCaseLetter = function (codePoint) { return codePoint >= A && codePoint <= Z; };
    var isLetter = function (codePoint) { return isLowerCaseLetter(codePoint) || isUpperCaseLetter(codePoint); };
    var isNonASCIICodePoint = function (codePoint) { return codePoint >= CONTROL; };
    var isWhiteSpace = function (codePoint) {
        return codePoint === LINE_FEED || codePoint === CHARACTER_TABULATION || codePoint === SPACE;
    };
    var isNameStartCodePoint = function (codePoint) {
        return isLetter(codePoint) || isNonASCIICodePoint(codePoint) || codePoint === LOW_LINE;
    };
    var isNameCodePoint = function (codePoint) {
        return isNameStartCodePoint(codePoint) || isDigit(codePoint) || codePoint === HYPHEN_MINUS;
    };
    var isNonPrintableCodePoint = function (codePoint) {
        return ((codePoint >= NULL && codePoint <= BACKSPACE) ||
            codePoint === LINE_TABULATION ||
            (codePoint >= SHIFT_OUT && codePoint <= INFORMATION_SEPARATOR_ONE) ||
            codePoint === DELETE);
    };
    var isValidEscape = function (c1, c2) {
        if (c1 !== REVERSE_SOLIDUS) {
            return false;
        }
        return c2 !== LINE_FEED;
    };
    var isIdentifierStart = function (c1, c2, c3) {
        if (c1 === HYPHEN_MINUS) {
            return isNameStartCodePoint(c2) || isValidEscape(c2, c3);
        }
        else if (isNameStartCodePoint(c1)) {
            return true;
        }
        else if (c1 === REVERSE_SOLIDUS && isValidEscape(c1, c2)) {
            return true;
        }
        return false;
    };
    var isNumberStart = function (c1, c2, c3) {
        if (c1 === PLUS_SIGN || c1 === HYPHEN_MINUS) {
            if (isDigit(c2)) {
                return true;
            }
            return c2 === FULL_STOP && isDigit(c3);
        }
        if (c1 === FULL_STOP) {
            return isDigit(c2);
        }
        return isDigit(c1);
    };
    var stringToNumber = function (codePoints) {
        var c = 0;
        var sign = 1;
        if (codePoints[c] === PLUS_SIGN || codePoints[c] === HYPHEN_MINUS) {
            if (codePoints[c] === HYPHEN_MINUS) {
                sign = -1;
            }
            c++;
        }
        var integers = [];
        while (isDigit(codePoints[c])) {
            integers.push(codePoints[c++]);
        }
        var int = integers.length ? parseInt(fromCodePoint$1.apply(void 0, integers), 10) : 0;
        if (codePoints[c] === FULL_STOP) {
            c++;
        }
        var fraction = [];
        while (isDigit(codePoints[c])) {
            fraction.push(codePoints[c++]);
        }
        var fracd = fraction.length;
        var frac = fracd ? parseInt(fromCodePoint$1.apply(void 0, fraction), 10) : 0;
        if (codePoints[c] === E || codePoints[c] === e) {
            c++;
        }
        var expsign = 1;
        if (codePoints[c] === PLUS_SIGN || codePoints[c] === HYPHEN_MINUS) {
            if (codePoints[c] === HYPHEN_MINUS) {
                expsign = -1;
            }
            c++;
        }
        var exponent = [];
        while (isDigit(codePoints[c])) {
            exponent.push(codePoints[c++]);
        }
        var exp = exponent.length ? parseInt(fromCodePoint$1.apply(void 0, exponent), 10) : 0;
        return sign * (int + frac * Math.pow(10, -fracd)) * Math.pow(10, expsign * exp);
    };
    var LEFT_PARENTHESIS_TOKEN = {
        type: 2 /* LEFT_PARENTHESIS_TOKEN */
    };
    var RIGHT_PARENTHESIS_TOKEN = {
        type: 3 /* RIGHT_PARENTHESIS_TOKEN */
    };
    var COMMA_TOKEN = { type: 4 /* COMMA_TOKEN */ };
    var SUFFIX_MATCH_TOKEN = { type: 13 /* SUFFIX_MATCH_TOKEN */ };
    var PREFIX_MATCH_TOKEN = { type: 8 /* PREFIX_MATCH_TOKEN */ };
    var COLUMN_TOKEN = { type: 21 /* COLUMN_TOKEN */ };
    var DASH_MATCH_TOKEN = { type: 9 /* DASH_MATCH_TOKEN */ };
    var INCLUDE_MATCH_TOKEN = { type: 10 /* INCLUDE_MATCH_TOKEN */ };
    var LEFT_CURLY_BRACKET_TOKEN = {
        type: 11 /* LEFT_CURLY_BRACKET_TOKEN */
    };
    var RIGHT_CURLY_BRACKET_TOKEN = {
        type: 12 /* RIGHT_CURLY_BRACKET_TOKEN */
    };
    var SUBSTRING_MATCH_TOKEN = { type: 14 /* SUBSTRING_MATCH_TOKEN */ };
    var BAD_URL_TOKEN = { type: 23 /* BAD_URL_TOKEN */ };
    var BAD_STRING_TOKEN = { type: 1 /* BAD_STRING_TOKEN */ };
    var CDO_TOKEN = { type: 25 /* CDO_TOKEN */ };
    var CDC_TOKEN = { type: 24 /* CDC_TOKEN */ };
    var COLON_TOKEN = { type: 26 /* COLON_TOKEN */ };
    var SEMICOLON_TOKEN = { type: 27 /* SEMICOLON_TOKEN */ };
    var LEFT_SQUARE_BRACKET_TOKEN = {
        type: 28 /* LEFT_SQUARE_BRACKET_TOKEN */
    };
    var RIGHT_SQUARE_BRACKET_TOKEN = {
        type: 29 /* RIGHT_SQUARE_BRACKET_TOKEN */
    };
    var WHITESPACE_TOKEN = { type: 31 /* WHITESPACE_TOKEN */ };
    var EOF_TOKEN = { type: 32 /* EOF_TOKEN */ };
    var Tokenizer = /** @class */ (function () {
        function Tokenizer() {
            this._value = [];
        }
        Tokenizer.prototype.write = function (chunk) {
            this._value = this._value.concat(toCodePoints$1(chunk));
        };
        Tokenizer.prototype.read = function () {
            var tokens = [];
            var token = this.consumeToken();
            while (token !== EOF_TOKEN) {
                tokens.push(token);
                token = this.consumeToken();
            }
            return tokens;
        };
        Tokenizer.prototype.consumeToken = function () {
            var codePoint = this.consumeCodePoint();
            switch (codePoint) {
                case QUOTATION_MARK:
                    return this.consumeStringToken(QUOTATION_MARK);
                case NUMBER_SIGN:
                    var c1 = this.peekCodePoint(0);
                    var c2 = this.peekCodePoint(1);
                    var c3 = this.peekCodePoint(2);
                    if (isNameCodePoint(c1) || isValidEscape(c2, c3)) {
                        var flags = isIdentifierStart(c1, c2, c3) ? FLAG_ID : FLAG_UNRESTRICTED;
                        var value = this.consumeName();
                        return { type: 5 /* HASH_TOKEN */, value: value, flags: flags };
                    }
                    break;
                case DOLLAR_SIGN:
                    if (this.peekCodePoint(0) === EQUALS_SIGN) {
                        this.consumeCodePoint();
                        return SUFFIX_MATCH_TOKEN;
                    }
                    break;
                case APOSTROPHE:
                    return this.consumeStringToken(APOSTROPHE);
                case LEFT_PARENTHESIS:
                    return LEFT_PARENTHESIS_TOKEN;
                case RIGHT_PARENTHESIS:
                    return RIGHT_PARENTHESIS_TOKEN;
                case ASTERISK:
                    if (this.peekCodePoint(0) === EQUALS_SIGN) {
                        this.consumeCodePoint();
                        return SUBSTRING_MATCH_TOKEN;
                    }
                    break;
                case PLUS_SIGN:
                    if (isNumberStart(codePoint, this.peekCodePoint(0), this.peekCodePoint(1))) {
                        this.reconsumeCodePoint(codePoint);
                        return this.consumeNumericToken();
                    }
                    break;
                case COMMA:
                    return COMMA_TOKEN;
                case HYPHEN_MINUS:
                    var e1 = codePoint;
                    var e2 = this.peekCodePoint(0);
                    var e3 = this.peekCodePoint(1);
                    if (isNumberStart(e1, e2, e3)) {
                        this.reconsumeCodePoint(codePoint);
                        return this.consumeNumericToken();
                    }
                    if (isIdentifierStart(e1, e2, e3)) {
                        this.reconsumeCodePoint(codePoint);
                        return this.consumeIdentLikeToken();
                    }
                    if (e2 === HYPHEN_MINUS && e3 === GREATER_THAN_SIGN) {
                        this.consumeCodePoint();
                        this.consumeCodePoint();
                        return CDC_TOKEN;
                    }
                    break;
                case FULL_STOP:
                    if (isNumberStart(codePoint, this.peekCodePoint(0), this.peekCodePoint(1))) {
                        this.reconsumeCodePoint(codePoint);
                        return this.consumeNumericToken();
                    }
                    break;
                case SOLIDUS:
                    if (this.peekCodePoint(0) === ASTERISK) {
                        this.consumeCodePoint();
                        while (true) {
                            var c = this.consumeCodePoint();
                            if (c === ASTERISK) {
                                c = this.consumeCodePoint();
                                if (c === SOLIDUS) {
                                    return this.consumeToken();
                                }
                            }
                            if (c === EOF) {
                                return this.consumeToken();
                            }
                        }
                    }
                    break;
                case COLON:
                    return COLON_TOKEN;
                case SEMICOLON:
                    return SEMICOLON_TOKEN;
                case LESS_THAN_SIGN:
                    if (this.peekCodePoint(0) === EXCLAMATION_MARK &&
                        this.peekCodePoint(1) === HYPHEN_MINUS &&
                        this.peekCodePoint(2) === HYPHEN_MINUS) {
                        this.consumeCodePoint();
                        this.consumeCodePoint();
                        return CDO_TOKEN;
                    }
                    break;
                case COMMERCIAL_AT:
                    var a1 = this.peekCodePoint(0);
                    var a2 = this.peekCodePoint(1);
                    var a3 = this.peekCodePoint(2);
                    if (isIdentifierStart(a1, a2, a3)) {
                        var value = this.consumeName();
                        return { type: 7 /* AT_KEYWORD_TOKEN */, value: value };
                    }
                    break;
                case LEFT_SQUARE_BRACKET:
                    return LEFT_SQUARE_BRACKET_TOKEN;
                case REVERSE_SOLIDUS:
                    if (isValidEscape(codePoint, this.peekCodePoint(0))) {
                        this.reconsumeCodePoint(codePoint);
                        return this.consumeIdentLikeToken();
                    }
                    break;
                case RIGHT_SQUARE_BRACKET:
                    return RIGHT_SQUARE_BRACKET_TOKEN;
                case CIRCUMFLEX_ACCENT:
                    if (this.peekCodePoint(0) === EQUALS_SIGN) {
                        this.consumeCodePoint();
                        return PREFIX_MATCH_TOKEN;
                    }
                    break;
                case LEFT_CURLY_BRACKET:
                    return LEFT_CURLY_BRACKET_TOKEN;
                case RIGHT_CURLY_BRACKET:
                    return RIGHT_CURLY_BRACKET_TOKEN;
                case u:
                case U:
                    var u1 = this.peekCodePoint(0);
                    var u2 = this.peekCodePoint(1);
                    if (u1 === PLUS_SIGN && (isHex(u2) || u2 === QUESTION_MARK)) {
                        this.consumeCodePoint();
                        this.consumeUnicodeRangeToken();
                    }
                    this.reconsumeCodePoint(codePoint);
                    return this.consumeIdentLikeToken();
                case VERTICAL_LINE:
                    if (this.peekCodePoint(0) === EQUALS_SIGN) {
                        this.consumeCodePoint();
                        return DASH_MATCH_TOKEN;
                    }
                    if (this.peekCodePoint(0) === VERTICAL_LINE) {
                        this.consumeCodePoint();
                        return COLUMN_TOKEN;
                    }
                    break;
                case TILDE:
                    if (this.peekCodePoint(0) === EQUALS_SIGN) {
                        this.consumeCodePoint();
                        return INCLUDE_MATCH_TOKEN;
                    }
                    break;
                case EOF:
                    return EOF_TOKEN;
            }
            if (isWhiteSpace(codePoint)) {
                this.consumeWhiteSpace();
                return WHITESPACE_TOKEN;
            }
            if (isDigit(codePoint)) {
                this.reconsumeCodePoint(codePoint);
                return this.consumeNumericToken();
            }
            if (isNameStartCodePoint(codePoint)) {
                this.reconsumeCodePoint(codePoint);
                return this.consumeIdentLikeToken();
            }
            return { type: 6 /* DELIM_TOKEN */, value: fromCodePoint$1(codePoint) };
        };
        Tokenizer.prototype.consumeCodePoint = function () {
            var value = this._value.shift();
            return typeof value === 'undefined' ? -1 : value;
        };
        Tokenizer.prototype.reconsumeCodePoint = function (codePoint) {
            this._value.unshift(codePoint);
        };
        Tokenizer.prototype.peekCodePoint = function (delta) {
            if (delta >= this._value.length) {
                return -1;
            }
            return this._value[delta];
        };
        Tokenizer.prototype.consumeUnicodeRangeToken = function () {
            var digits = [];
            var codePoint = this.consumeCodePoint();
            while (isHex(codePoint) && digits.length < 6) {
                digits.push(codePoint);
                codePoint = this.consumeCodePoint();
            }
            var questionMarks = false;
            while (codePoint === QUESTION_MARK && digits.length < 6) {
                digits.push(codePoint);
                codePoint = this.consumeCodePoint();
                questionMarks = true;
            }
            if (questionMarks) {
                var start_1 = parseInt(fromCodePoint$1.apply(void 0, digits.map(function (digit) { return (digit === QUESTION_MARK ? ZERO : digit); })), 16);
                var end = parseInt(fromCodePoint$1.apply(void 0, digits.map(function (digit) { return (digit === QUESTION_MARK ? F : digit); })), 16);
                return { type: 30 /* UNICODE_RANGE_TOKEN */, start: start_1, end: end };
            }
            var start = parseInt(fromCodePoint$1.apply(void 0, digits), 16);
            if (this.peekCodePoint(0) === HYPHEN_MINUS && isHex(this.peekCodePoint(1))) {
                this.consumeCodePoint();
                codePoint = this.consumeCodePoint();
                var endDigits = [];
                while (isHex(codePoint) && endDigits.length < 6) {
                    endDigits.push(codePoint);
                    codePoint = this.consumeCodePoint();
                }
                var end = parseInt(fromCodePoint$1.apply(void 0, endDigits), 16);
                return { type: 30 /* UNICODE_RANGE_TOKEN */, start: start, end: end };
            }
            else {
                return { type: 30 /* UNICODE_RANGE_TOKEN */, start: start, end: start };
            }
        };
        Tokenizer.prototype.consumeIdentLikeToken = function () {
            var value = this.consumeName();
            if (value.toLowerCase() === 'url' && this.peekCodePoint(0) === LEFT_PARENTHESIS) {
                this.consumeCodePoint();
                return this.consumeUrlToken();
            }
            else if (this.peekCodePoint(0) === LEFT_PARENTHESIS) {
                this.consumeCodePoint();
                return { type: 19 /* FUNCTION_TOKEN */, value: value };
            }
            return { type: 20 /* IDENT_TOKEN */, value: value };
        };
        Tokenizer.prototype.consumeUrlToken = function () {
            var value = [];
            this.consumeWhiteSpace();
            if (this.peekCodePoint(0) === EOF) {
                return { type: 22 /* URL_TOKEN */, value: '' };
            }
            var next = this.peekCodePoint(0);
            if (next === APOSTROPHE || next === QUOTATION_MARK) {
                var stringToken = this.consumeStringToken(this.consumeCodePoint());
                if (stringToken.type === 0 /* STRING_TOKEN */) {
                    this.consumeWhiteSpace();
                    if (this.peekCodePoint(0) === EOF || this.peekCodePoint(0) === RIGHT_PARENTHESIS) {
                        this.consumeCodePoint();
                        return { type: 22 /* URL_TOKEN */, value: stringToken.value };
                    }
                }
                this.consumeBadUrlRemnants();
                return BAD_URL_TOKEN;
            }
            while (true) {
                var codePoint = this.consumeCodePoint();
                if (codePoint === EOF || codePoint === RIGHT_PARENTHESIS) {
                    return { type: 22 /* URL_TOKEN */, value: fromCodePoint$1.apply(void 0, value) };
                }
                else if (isWhiteSpace(codePoint)) {
                    this.consumeWhiteSpace();
                    if (this.peekCodePoint(0) === EOF || this.peekCodePoint(0) === RIGHT_PARENTHESIS) {
                        this.consumeCodePoint();
                        return { type: 22 /* URL_TOKEN */, value: fromCodePoint$1.apply(void 0, value) };
                    }
                    this.consumeBadUrlRemnants();
                    return BAD_URL_TOKEN;
                }
                else if (codePoint === QUOTATION_MARK ||
                    codePoint === APOSTROPHE ||
                    codePoint === LEFT_PARENTHESIS ||
                    isNonPrintableCodePoint(codePoint)) {
                    this.consumeBadUrlRemnants();
                    return BAD_URL_TOKEN;
                }
                else if (codePoint === REVERSE_SOLIDUS) {
                    if (isValidEscape(codePoint, this.peekCodePoint(0))) {
                        value.push(this.consumeEscapedCodePoint());
                    }
                    else {
                        this.consumeBadUrlRemnants();
                        return BAD_URL_TOKEN;
                    }
                }
                else {
                    value.push(codePoint);
                }
            }
        };
        Tokenizer.prototype.consumeWhiteSpace = function () {
            while (isWhiteSpace(this.peekCodePoint(0))) {
                this.consumeCodePoint();
            }
        };
        Tokenizer.prototype.consumeBadUrlRemnants = function () {
            while (true) {
                var codePoint = this.consumeCodePoint();
                if (codePoint === RIGHT_PARENTHESIS || codePoint === EOF) {
                    return;
                }
                if (isValidEscape(codePoint, this.peekCodePoint(0))) {
                    this.consumeEscapedCodePoint();
                }
            }
        };
        Tokenizer.prototype.consumeStringSlice = function (count) {
            var SLICE_STACK_SIZE = 50000;
            var value = '';
            while (count > 0) {
                var amount = Math.min(SLICE_STACK_SIZE, count);
                value += fromCodePoint$1.apply(void 0, this._value.splice(0, amount));
                count -= amount;
            }
            this._value.shift();
            return value;
        };
        Tokenizer.prototype.consumeStringToken = function (endingCodePoint) {
            var value = '';
            var i = 0;
            do {
                var codePoint = this._value[i];
                if (codePoint === EOF || codePoint === undefined || codePoint === endingCodePoint) {
                    value += this.consumeStringSlice(i);
                    return { type: 0 /* STRING_TOKEN */, value: value };
                }
                if (codePoint === LINE_FEED) {
                    this._value.splice(0, i);
                    return BAD_STRING_TOKEN;
                }
                if (codePoint === REVERSE_SOLIDUS) {
                    var next = this._value[i + 1];
                    if (next !== EOF && next !== undefined) {
                        if (next === LINE_FEED) {
                            value += this.consumeStringSlice(i);
                            i = -1;
                            this._value.shift();
                        }
                        else if (isValidEscape(codePoint, next)) {
                            value += this.consumeStringSlice(i);
                            value += fromCodePoint$1(this.consumeEscapedCodePoint());
                            i = -1;
                        }
                    }
                }
                i++;
            } while (true);
        };
        Tokenizer.prototype.consumeNumber = function () {
            var repr = [];
            var type = FLAG_INTEGER;
            var c1 = this.peekCodePoint(0);
            if (c1 === PLUS_SIGN || c1 === HYPHEN_MINUS) {
                repr.push(this.consumeCodePoint());
            }
            while (isDigit(this.peekCodePoint(0))) {
                repr.push(this.consumeCodePoint());
            }
            c1 = this.peekCodePoint(0);
            var c2 = this.peekCodePoint(1);
            if (c1 === FULL_STOP && isDigit(c2)) {
                repr.push(this.consumeCodePoint(), this.consumeCodePoint());
                type = FLAG_NUMBER;
                while (isDigit(this.peekCodePoint(0))) {
                    repr.push(this.consumeCodePoint());
                }
            }
            c1 = this.peekCodePoint(0);
            c2 = this.peekCodePoint(1);
            var c3 = this.peekCodePoint(2);
            if ((c1 === E || c1 === e) && (((c2 === PLUS_SIGN || c2 === HYPHEN_MINUS) && isDigit(c3)) || isDigit(c2))) {
                repr.push(this.consumeCodePoint(), this.consumeCodePoint());
                type = FLAG_NUMBER;
                while (isDigit(this.peekCodePoint(0))) {
                    repr.push(this.consumeCodePoint());
                }
            }
            return [stringToNumber(repr), type];
        };
        Tokenizer.prototype.consumeNumericToken = function () {
            var _a = this.consumeNumber(), number = _a[0], flags = _a[1];
            var c1 = this.peekCodePoint(0);
            var c2 = this.peekCodePoint(1);
            var c3 = this.peekCodePoint(2);
            if (isIdentifierStart(c1, c2, c3)) {
                var unit = this.consumeName();
                return { type: 15 /* DIMENSION_TOKEN */, number: number, flags: flags, unit: unit };
            }
            if (c1 === PERCENTAGE_SIGN) {
                this.consumeCodePoint();
                return { type: 16 /* PERCENTAGE_TOKEN */, number: number, flags: flags };
            }
            return { type: 17 /* NUMBER_TOKEN */, number: number, flags: flags };
        };
        Tokenizer.prototype.consumeEscapedCodePoint = function () {
            var codePoint = this.consumeCodePoint();
            if (isHex(codePoint)) {
                var hex = fromCodePoint$1(codePoint);
                while (isHex(this.peekCodePoint(0)) && hex.length < 6) {
                    hex += fromCodePoint$1(this.consumeCodePoint());
                }
                if (isWhiteSpace(this.peekCodePoint(0))) {
                    this.consumeCodePoint();
                }
                var hexCodePoint = parseInt(hex, 16);
                if (hexCodePoint === 0 || isSurrogateCodePoint(hexCodePoint) || hexCodePoint > 0x10ffff) {
                    return REPLACEMENT_CHARACTER;
                }
                return hexCodePoint;
            }
            if (codePoint === EOF) {
                return REPLACEMENT_CHARACTER;
            }
            return codePoint;
        };
        Tokenizer.prototype.consumeName = function () {
            var result = '';
            while (true) {
                var codePoint = this.consumeCodePoint();
                if (isNameCodePoint(codePoint)) {
                    result += fromCodePoint$1(codePoint);
                }
                else if (isValidEscape(codePoint, this.peekCodePoint(0))) {
                    result += fromCodePoint$1(this.consumeEscapedCodePoint());
                }
                else {
                    this.reconsumeCodePoint(codePoint);
                    return result;
                }
            }
        };
        return Tokenizer;
    }());

    var Parser = /** @class */ (function () {
        function Parser(tokens) {
            this._tokens = tokens;
        }
        Parser.create = function (value) {
            var tokenizer = new Tokenizer();
            tokenizer.write(value);
            return new Parser(tokenizer.read());
        };
        Parser.parseValue = function (value) {
            return Parser.create(value).parseComponentValue();
        };
        Parser.parseValues = function (value) {
            return Parser.create(value).parseComponentValues();
        };
        Parser.prototype.parseComponentValue = function () {
            var token = this.consumeToken();
            while (token.type === 31 /* WHITESPACE_TOKEN */) {
                token = this.consumeToken();
            }
            if (token.type === 32 /* EOF_TOKEN */) {
                throw new SyntaxError("Error parsing CSS component value, unexpected EOF");
            }
            this.reconsumeToken(token);
            var value = this.consumeComponentValue();
            do {
                token = this.consumeToken();
            } while (token.type === 31 /* WHITESPACE_TOKEN */);
            if (token.type === 32 /* EOF_TOKEN */) {
                return value;
            }
            throw new SyntaxError("Error parsing CSS component value, multiple values found when expecting only one");
        };
        Parser.prototype.parseComponentValues = function () {
            var values = [];
            while (true) {
                var value = this.consumeComponentValue();
                if (value.type === 32 /* EOF_TOKEN */) {
                    return values;
                }
                values.push(value);
                values.push();
            }
        };
        Parser.prototype.consumeComponentValue = function () {
            var token = this.consumeToken();
            switch (token.type) {
                case 11 /* LEFT_CURLY_BRACKET_TOKEN */:
                case 28 /* LEFT_SQUARE_BRACKET_TOKEN */:
                case 2 /* LEFT_PARENTHESIS_TOKEN */:
                    return this.consumeSimpleBlock(token.type);
                case 19 /* FUNCTION_TOKEN */:
                    return this.consumeFunction(token);
            }
            return token;
        };
        Parser.prototype.consumeSimpleBlock = function (type) {
            var block = { type: type, values: [] };
            var token = this.consumeToken();
            while (true) {
                if (token.type === 32 /* EOF_TOKEN */ || isEndingTokenFor(token, type)) {
                    return block;
                }
                this.reconsumeToken(token);
                block.values.push(this.consumeComponentValue());
                token = this.consumeToken();
            }
        };
        Parser.prototype.consumeFunction = function (functionToken) {
            var cssFunction = {
                name: functionToken.value,
                values: [],
                type: 18 /* FUNCTION */
            };
            while (true) {
                var token = this.consumeToken();
                if (token.type === 32 /* EOF_TOKEN */ || token.type === 3 /* RIGHT_PARENTHESIS_TOKEN */) {
                    return cssFunction;
                }
                this.reconsumeToken(token);
                cssFunction.values.push(this.consumeComponentValue());
            }
        };
        Parser.prototype.consumeToken = function () {
            var token = this._tokens.shift();
            return typeof token === 'undefined' ? EOF_TOKEN : token;
        };
        Parser.prototype.reconsumeToken = function (token) {
            this._tokens.unshift(token);
        };
        return Parser;
    }());
    var isDimensionToken = function (token) { return token.type === 15 /* DIMENSION_TOKEN */; };
    var isNumberToken = function (token) { return token.type === 17 /* NUMBER_TOKEN */; };
    var isIdentToken = function (token) { return token.type === 20 /* IDENT_TOKEN */; };
    var isStringToken = function (token) { return token.type === 0 /* STRING_TOKEN */; };
    var isIdentWithValue = function (token, value) {
        return isIdentToken(token) && token.value === value;
    };
    var nonWhiteSpace = function (token) { return token.type !== 31 /* WHITESPACE_TOKEN */; };
    var nonFunctionArgSeparator = function (token) {
        return token.type !== 31 /* WHITESPACE_TOKEN */ && token.type !== 4 /* COMMA_TOKEN */;
    };
    var parseFunctionArgs = function (tokens) {
        var args = [];
        var arg = [];
        tokens.forEach(function (token) {
            if (token.type === 4 /* COMMA_TOKEN */) {
                if (arg.length === 0) {
                    throw new Error("Error parsing function args, zero tokens for arg");
                }
                args.push(arg);
                arg = [];
                return;
            }
            if (token.type !== 31 /* WHITESPACE_TOKEN */) {
                arg.push(token);
            }
        });
        if (arg.length) {
            args.push(arg);
        }
        return args;
    };
    var isEndingTokenFor = function (token, type) {
        if (type === 11 /* LEFT_CURLY_BRACKET_TOKEN */ && token.type === 12 /* RIGHT_CURLY_BRACKET_TOKEN */) {
            return true;
        }
        if (type === 28 /* LEFT_SQUARE_BRACKET_TOKEN */ && token.type === 29 /* RIGHT_SQUARE_BRACKET_TOKEN */) {
            return true;
        }
        return type === 2 /* LEFT_PARENTHESIS_TOKEN */ && token.type === 3 /* RIGHT_PARENTHESIS_TOKEN */;
    };

    var isLength = function (token) {
        return token.type === 17 /* NUMBER_TOKEN */ || token.type === 15 /* DIMENSION_TOKEN */;
    };

    var isLengthPercentage = function (token) {
        return token.type === 16 /* PERCENTAGE_TOKEN */ || isLength(token);
    };
    var parseLengthPercentageTuple = function (tokens) {
        return tokens.length > 1 ? [tokens[0], tokens[1]] : [tokens[0]];
    };
    var ZERO_LENGTH = {
        type: 17 /* NUMBER_TOKEN */,
        number: 0,
        flags: FLAG_INTEGER
    };
    var FIFTY_PERCENT = {
        type: 16 /* PERCENTAGE_TOKEN */,
        number: 50,
        flags: FLAG_INTEGER
    };
    var HUNDRED_PERCENT = {
        type: 16 /* PERCENTAGE_TOKEN */,
        number: 100,
        flags: FLAG_INTEGER
    };
    var getAbsoluteValueForTuple = function (tuple, width, height) {
        var x = tuple[0], y = tuple[1];
        return [getAbsoluteValue(x, width), getAbsoluteValue(typeof y !== 'undefined' ? y : x, height)];
    };
    var getAbsoluteValue = function (token, parent) {
        if (token.type === 16 /* PERCENTAGE_TOKEN */) {
            return (token.number / 100) * parent;
        }
        if (isDimensionToken(token)) {
            switch (token.unit) {
                case 'rem':
                case 'em':
                    return 16 * token.number; // TODO use correct font-size
                case 'px':
                default:
                    return token.number;
            }
        }
        return token.number;
    };

    var DEG = 'deg';
    var GRAD = 'grad';
    var RAD = 'rad';
    var TURN = 'turn';
    var angle = {
        name: 'angle',
        parse: function (_context, value) {
            if (value.type === 15 /* DIMENSION_TOKEN */) {
                switch (value.unit) {
                    case DEG:
                        return (Math.PI * value.number) / 180;
                    case GRAD:
                        return (Math.PI / 200) * value.number;
                    case RAD:
                        return value.number;
                    case TURN:
                        return Math.PI * 2 * value.number;
                }
            }
            throw new Error("Unsupported angle type");
        }
    };
    var isAngle = function (value) {
        if (value.type === 15 /* DIMENSION_TOKEN */) {
            if (value.unit === DEG || value.unit === GRAD || value.unit === RAD || value.unit === TURN) {
                return true;
            }
        }
        return false;
    };
    var parseNamedSide = function (tokens) {
        var sideOrCorner = tokens
            .filter(isIdentToken)
            .map(function (ident) { return ident.value; })
            .join(' ');
        switch (sideOrCorner) {
            case 'to bottom right':
            case 'to right bottom':
            case 'left top':
            case 'top left':
                return [ZERO_LENGTH, ZERO_LENGTH];
            case 'to top':
            case 'bottom':
                return deg(0);
            case 'to bottom left':
            case 'to left bottom':
            case 'right top':
            case 'top right':
                return [ZERO_LENGTH, HUNDRED_PERCENT];
            case 'to right':
            case 'left':
                return deg(90);
            case 'to top left':
            case 'to left top':
            case 'right bottom':
            case 'bottom right':
                return [HUNDRED_PERCENT, HUNDRED_PERCENT];
            case 'to bottom':
            case 'top':
                return deg(180);
            case 'to top right':
            case 'to right top':
            case 'left bottom':
            case 'bottom left':
                return [HUNDRED_PERCENT, ZERO_LENGTH];
            case 'to left':
            case 'right':
                return deg(270);
        }
        return 0;
    };
    var deg = function (deg) { return (Math.PI * deg) / 180; };

    var color$1 = {
        name: 'color',
        parse: function (context, value) {
            if (value.type === 18 /* FUNCTION */) {
                var colorFunction = SUPPORTED_COLOR_FUNCTIONS[value.name];
                if (typeof colorFunction === 'undefined') {
                    throw new Error("Attempting to parse an unsupported color function \"" + value.name + "\"");
                }
                return colorFunction(context, value.values);
            }
            if (value.type === 5 /* HASH_TOKEN */) {
                if (value.value.length === 3) {
                    var r = value.value.substring(0, 1);
                    var g = value.value.substring(1, 2);
                    var b = value.value.substring(2, 3);
                    return pack(parseInt(r + r, 16), parseInt(g + g, 16), parseInt(b + b, 16), 1);
                }
                if (value.value.length === 4) {
                    var r = value.value.substring(0, 1);
                    var g = value.value.substring(1, 2);
                    var b = value.value.substring(2, 3);
                    var a = value.value.substring(3, 4);
                    return pack(parseInt(r + r, 16), parseInt(g + g, 16), parseInt(b + b, 16), parseInt(a + a, 16) / 255);
                }
                if (value.value.length === 6) {
                    var r = value.value.substring(0, 2);
                    var g = value.value.substring(2, 4);
                    var b = value.value.substring(4, 6);
                    return pack(parseInt(r, 16), parseInt(g, 16), parseInt(b, 16), 1);
                }
                if (value.value.length === 8) {
                    var r = value.value.substring(0, 2);
                    var g = value.value.substring(2, 4);
                    var b = value.value.substring(4, 6);
                    var a = value.value.substring(6, 8);
                    return pack(parseInt(r, 16), parseInt(g, 16), parseInt(b, 16), parseInt(a, 16) / 255);
                }
            }
            if (value.type === 20 /* IDENT_TOKEN */) {
                var namedColor = COLORS[value.value.toUpperCase()];
                if (typeof namedColor !== 'undefined') {
                    return namedColor;
                }
            }
            return COLORS.TRANSPARENT;
        }
    };
    var isTransparent = function (color) { return (0xff & color) === 0; };
    var asString = function (color) {
        var alpha = 0xff & color;
        var blue = 0xff & (color >> 8);
        var green = 0xff & (color >> 16);
        var red = 0xff & (color >> 24);
        return alpha < 255 ? "rgba(" + red + "," + green + "," + blue + "," + alpha / 255 + ")" : "rgb(" + red + "," + green + "," + blue + ")";
    };
    var pack = function (r, g, b, a) {
        return ((r << 24) | (g << 16) | (b << 8) | (Math.round(a * 255) << 0)) >>> 0;
    };
    var getTokenColorValue = function (token, i) {
        if (token.type === 17 /* NUMBER_TOKEN */) {
            return token.number;
        }
        if (token.type === 16 /* PERCENTAGE_TOKEN */) {
            var max = i === 3 ? 1 : 255;
            return i === 3 ? (token.number / 100) * max : Math.round((token.number / 100) * max);
        }
        return 0;
    };
    var rgb = function (_context, args) {
        var tokens = args.filter(nonFunctionArgSeparator);
        if (tokens.length === 3) {
            var _a = tokens.map(getTokenColorValue), r = _a[0], g = _a[1], b = _a[2];
            return pack(r, g, b, 1);
        }
        if (tokens.length === 4) {
            var _b = tokens.map(getTokenColorValue), r = _b[0], g = _b[1], b = _b[2], a = _b[3];
            return pack(r, g, b, a);
        }
        return 0;
    };
    function hue2rgb(t1, t2, hue) {
        if (hue < 0) {
            hue += 1;
        }
        if (hue >= 1) {
            hue -= 1;
        }
        if (hue < 1 / 6) {
            return (t2 - t1) * hue * 6 + t1;
        }
        else if (hue < 1 / 2) {
            return t2;
        }
        else if (hue < 2 / 3) {
            return (t2 - t1) * 6 * (2 / 3 - hue) + t1;
        }
        else {
            return t1;
        }
    }
    var hsl = function (context, args) {
        var tokens = args.filter(nonFunctionArgSeparator);
        var hue = tokens[0], saturation = tokens[1], lightness = tokens[2], alpha = tokens[3];
        var h = (hue.type === 17 /* NUMBER_TOKEN */ ? deg(hue.number) : angle.parse(context, hue)) / (Math.PI * 2);
        var s = isLengthPercentage(saturation) ? saturation.number / 100 : 0;
        var l = isLengthPercentage(lightness) ? lightness.number / 100 : 0;
        var a = typeof alpha !== 'undefined' && isLengthPercentage(alpha) ? getAbsoluteValue(alpha, 1) : 1;
        if (s === 0) {
            return pack(l * 255, l * 255, l * 255, 1);
        }
        var t2 = l <= 0.5 ? l * (s + 1) : l + s - l * s;
        var t1 = l * 2 - t2;
        var r = hue2rgb(t1, t2, h + 1 / 3);
        var g = hue2rgb(t1, t2, h);
        var b = hue2rgb(t1, t2, h - 1 / 3);
        return pack(r * 255, g * 255, b * 255, a);
    };
    var SUPPORTED_COLOR_FUNCTIONS = {
        hsl: hsl,
        hsla: hsl,
        rgb: rgb,
        rgba: rgb
    };
    var parseColor = function (context, value) {
        return color$1.parse(context, Parser.create(value).parseComponentValue());
    };
    var COLORS = {
        ALICEBLUE: 0xf0f8ffff,
        ANTIQUEWHITE: 0xfaebd7ff,
        AQUA: 0x00ffffff,
        AQUAMARINE: 0x7fffd4ff,
        AZURE: 0xf0ffffff,
        BEIGE: 0xf5f5dcff,
        BISQUE: 0xffe4c4ff,
        BLACK: 0x000000ff,
        BLANCHEDALMOND: 0xffebcdff,
        BLUE: 0x0000ffff,
        BLUEVIOLET: 0x8a2be2ff,
        BROWN: 0xa52a2aff,
        BURLYWOOD: 0xdeb887ff,
        CADETBLUE: 0x5f9ea0ff,
        CHARTREUSE: 0x7fff00ff,
        CHOCOLATE: 0xd2691eff,
        CORAL: 0xff7f50ff,
        CORNFLOWERBLUE: 0x6495edff,
        CORNSILK: 0xfff8dcff,
        CRIMSON: 0xdc143cff,
        CYAN: 0x00ffffff,
        DARKBLUE: 0x00008bff,
        DARKCYAN: 0x008b8bff,
        DARKGOLDENROD: 0xb886bbff,
        DARKGRAY: 0xa9a9a9ff,
        DARKGREEN: 0x006400ff,
        DARKGREY: 0xa9a9a9ff,
        DARKKHAKI: 0xbdb76bff,
        DARKMAGENTA: 0x8b008bff,
        DARKOLIVEGREEN: 0x556b2fff,
        DARKORANGE: 0xff8c00ff,
        DARKORCHID: 0x9932ccff,
        DARKRED: 0x8b0000ff,
        DARKSALMON: 0xe9967aff,
        DARKSEAGREEN: 0x8fbc8fff,
        DARKSLATEBLUE: 0x483d8bff,
        DARKSLATEGRAY: 0x2f4f4fff,
        DARKSLATEGREY: 0x2f4f4fff,
        DARKTURQUOISE: 0x00ced1ff,
        DARKVIOLET: 0x9400d3ff,
        DEEPPINK: 0xff1493ff,
        DEEPSKYBLUE: 0x00bfffff,
        DIMGRAY: 0x696969ff,
        DIMGREY: 0x696969ff,
        DODGERBLUE: 0x1e90ffff,
        FIREBRICK: 0xb22222ff,
        FLORALWHITE: 0xfffaf0ff,
        FORESTGREEN: 0x228b22ff,
        FUCHSIA: 0xff00ffff,
        GAINSBORO: 0xdcdcdcff,
        GHOSTWHITE: 0xf8f8ffff,
        GOLD: 0xffd700ff,
        GOLDENROD: 0xdaa520ff,
        GRAY: 0x808080ff,
        GREEN: 0x008000ff,
        GREENYELLOW: 0xadff2fff,
        GREY: 0x808080ff,
        HONEYDEW: 0xf0fff0ff,
        HOTPINK: 0xff69b4ff,
        INDIANRED: 0xcd5c5cff,
        INDIGO: 0x4b0082ff,
        IVORY: 0xfffff0ff,
        KHAKI: 0xf0e68cff,
        LAVENDER: 0xe6e6faff,
        LAVENDERBLUSH: 0xfff0f5ff,
        LAWNGREEN: 0x7cfc00ff,
        LEMONCHIFFON: 0xfffacdff,
        LIGHTBLUE: 0xadd8e6ff,
        LIGHTCORAL: 0xf08080ff,
        LIGHTCYAN: 0xe0ffffff,
        LIGHTGOLDENRODYELLOW: 0xfafad2ff,
        LIGHTGRAY: 0xd3d3d3ff,
        LIGHTGREEN: 0x90ee90ff,
        LIGHTGREY: 0xd3d3d3ff,
        LIGHTPINK: 0xffb6c1ff,
        LIGHTSALMON: 0xffa07aff,
        LIGHTSEAGREEN: 0x20b2aaff,
        LIGHTSKYBLUE: 0x87cefaff,
        LIGHTSLATEGRAY: 0x778899ff,
        LIGHTSLATEGREY: 0x778899ff,
        LIGHTSTEELBLUE: 0xb0c4deff,
        LIGHTYELLOW: 0xffffe0ff,
        LIME: 0x00ff00ff,
        LIMEGREEN: 0x32cd32ff,
        LINEN: 0xfaf0e6ff,
        MAGENTA: 0xff00ffff,
        MAROON: 0x800000ff,
        MEDIUMAQUAMARINE: 0x66cdaaff,
        MEDIUMBLUE: 0x0000cdff,
        MEDIUMORCHID: 0xba55d3ff,
        MEDIUMPURPLE: 0x9370dbff,
        MEDIUMSEAGREEN: 0x3cb371ff,
        MEDIUMSLATEBLUE: 0x7b68eeff,
        MEDIUMSPRINGGREEN: 0x00fa9aff,
        MEDIUMTURQUOISE: 0x48d1ccff,
        MEDIUMVIOLETRED: 0xc71585ff,
        MIDNIGHTBLUE: 0x191970ff,
        MINTCREAM: 0xf5fffaff,
        MISTYROSE: 0xffe4e1ff,
        MOCCASIN: 0xffe4b5ff,
        NAVAJOWHITE: 0xffdeadff,
        NAVY: 0x000080ff,
        OLDLACE: 0xfdf5e6ff,
        OLIVE: 0x808000ff,
        OLIVEDRAB: 0x6b8e23ff,
        ORANGE: 0xffa500ff,
        ORANGERED: 0xff4500ff,
        ORCHID: 0xda70d6ff,
        PALEGOLDENROD: 0xeee8aaff,
        PALEGREEN: 0x98fb98ff,
        PALETURQUOISE: 0xafeeeeff,
        PALEVIOLETRED: 0xdb7093ff,
        PAPAYAWHIP: 0xffefd5ff,
        PEACHPUFF: 0xffdab9ff,
        PERU: 0xcd853fff,
        PINK: 0xffc0cbff,
        PLUM: 0xdda0ddff,
        POWDERBLUE: 0xb0e0e6ff,
        PURPLE: 0x800080ff,
        REBECCAPURPLE: 0x663399ff,
        RED: 0xff0000ff,
        ROSYBROWN: 0xbc8f8fff,
        ROYALBLUE: 0x4169e1ff,
        SADDLEBROWN: 0x8b4513ff,
        SALMON: 0xfa8072ff,
        SANDYBROWN: 0xf4a460ff,
        SEAGREEN: 0x2e8b57ff,
        SEASHELL: 0xfff5eeff,
        SIENNA: 0xa0522dff,
        SILVER: 0xc0c0c0ff,
        SKYBLUE: 0x87ceebff,
        SLATEBLUE: 0x6a5acdff,
        SLATEGRAY: 0x708090ff,
        SLATEGREY: 0x708090ff,
        SNOW: 0xfffafaff,
        SPRINGGREEN: 0x00ff7fff,
        STEELBLUE: 0x4682b4ff,
        TAN: 0xd2b48cff,
        TEAL: 0x008080ff,
        THISTLE: 0xd8bfd8ff,
        TOMATO: 0xff6347ff,
        TRANSPARENT: 0x00000000,
        TURQUOISE: 0x40e0d0ff,
        VIOLET: 0xee82eeff,
        WHEAT: 0xf5deb3ff,
        WHITE: 0xffffffff,
        WHITESMOKE: 0xf5f5f5ff,
        YELLOW: 0xffff00ff,
        YELLOWGREEN: 0x9acd32ff
    };

    var backgroundClip = {
        name: 'background-clip',
        initialValue: 'border-box',
        prefix: false,
        type: 1 /* LIST */,
        parse: function (_context, tokens) {
            return tokens.map(function (token) {
                if (isIdentToken(token)) {
                    switch (token.value) {
                        case 'padding-box':
                            return 1 /* PADDING_BOX */;
                        case 'content-box':
                            return 2 /* CONTENT_BOX */;
                    }
                }
                return 0 /* BORDER_BOX */;
            });
        }
    };

    var backgroundColor = {
        name: "background-color",
        initialValue: 'transparent',
        prefix: false,
        type: 3 /* TYPE_VALUE */,
        format: 'color'
    };

    var parseColorStop = function (context, args) {
        var color = color$1.parse(context, args[0]);
        var stop = args[1];
        return stop && isLengthPercentage(stop) ? { color: color, stop: stop } : { color: color, stop: null };
    };
    var processColorStops = function (stops, lineLength) {
        var first = stops[0];
        var last = stops[stops.length - 1];
        if (first.stop === null) {
            first.stop = ZERO_LENGTH;
        }
        if (last.stop === null) {
            last.stop = HUNDRED_PERCENT;
        }
        var processStops = [];
        var previous = 0;
        for (var i = 0; i < stops.length; i++) {
            var stop_1 = stops[i].stop;
            if (stop_1 !== null) {
                var absoluteValue = getAbsoluteValue(stop_1, lineLength);
                if (absoluteValue > previous) {
                    processStops.push(absoluteValue);
                }
                else {
                    processStops.push(previous);
                }
                previous = absoluteValue;
            }
            else {
                processStops.push(null);
            }
        }
        var gapBegin = null;
        for (var i = 0; i < processStops.length; i++) {
            var stop_2 = processStops[i];
            if (stop_2 === null) {
                if (gapBegin === null) {
                    gapBegin = i;
                }
            }
            else if (gapBegin !== null) {
                var gapLength = i - gapBegin;
                var beforeGap = processStops[gapBegin - 1];
                var gapValue = (stop_2 - beforeGap) / (gapLength + 1);
                for (var g = 1; g <= gapLength; g++) {
                    processStops[gapBegin + g - 1] = gapValue * g;
                }
                gapBegin = null;
            }
        }
        return stops.map(function (_a, i) {
            var color = _a.color;
            return { color: color, stop: Math.max(Math.min(1, processStops[i] / lineLength), 0) };
        });
    };
    var getAngleFromCorner = function (corner, width, height) {
        var centerX = width / 2;
        var centerY = height / 2;
        var x = getAbsoluteValue(corner[0], width) - centerX;
        var y = centerY - getAbsoluteValue(corner[1], height);
        return (Math.atan2(y, x) + Math.PI * 2) % (Math.PI * 2);
    };
    var calculateGradientDirection = function (angle, width, height) {
        var radian = typeof angle === 'number' ? angle : getAngleFromCorner(angle, width, height);
        var lineLength = Math.abs(width * Math.sin(radian)) + Math.abs(height * Math.cos(radian));
        var halfWidth = width / 2;
        var halfHeight = height / 2;
        var halfLineLength = lineLength / 2;
        var yDiff = Math.sin(radian - Math.PI / 2) * halfLineLength;
        var xDiff = Math.cos(radian - Math.PI / 2) * halfLineLength;
        return [lineLength, halfWidth - xDiff, halfWidth + xDiff, halfHeight - yDiff, halfHeight + yDiff];
    };
    var distance = function (a, b) { return Math.sqrt(a * a + b * b); };
    var findCorner = function (width, height, x, y, closest) {
        var corners = [
            [0, 0],
            [0, height],
            [width, 0],
            [width, height]
        ];
        return corners.reduce(function (stat, corner) {
            var cx = corner[0], cy = corner[1];
            var d = distance(x - cx, y - cy);
            if (closest ? d < stat.optimumDistance : d > stat.optimumDistance) {
                return {
                    optimumCorner: corner,
                    optimumDistance: d
                };
            }
            return stat;
        }, {
            optimumDistance: closest ? Infinity : -Infinity,
            optimumCorner: null
        }).optimumCorner;
    };
    var calculateRadius = function (gradient, x, y, width, height) {
        var rx = 0;
        var ry = 0;
        switch (gradient.size) {
            case 0 /* CLOSEST_SIDE */:
                // The ending shape is sized so that that it exactly meets the side of the gradient box closest to the gradient’s center.
                // If the shape is an ellipse, it exactly meets the closest side in each dimension.
                if (gradient.shape === 0 /* CIRCLE */) {
                    rx = ry = Math.min(Math.abs(x), Math.abs(x - width), Math.abs(y), Math.abs(y - height));
                }
                else if (gradient.shape === 1 /* ELLIPSE */) {
                    rx = Math.min(Math.abs(x), Math.abs(x - width));
                    ry = Math.min(Math.abs(y), Math.abs(y - height));
                }
                break;
            case 2 /* CLOSEST_CORNER */:
                // The ending shape is sized so that that it passes through the corner of the gradient box closest to the gradient’s center.
                // If the shape is an ellipse, the ending shape is given the same aspect-ratio it would have if closest-side were specified.
                if (gradient.shape === 0 /* CIRCLE */) {
                    rx = ry = Math.min(distance(x, y), distance(x, y - height), distance(x - width, y), distance(x - width, y - height));
                }
                else if (gradient.shape === 1 /* ELLIPSE */) {
                    // Compute the ratio ry/rx (which is to be the same as for "closest-side")
                    var c = Math.min(Math.abs(y), Math.abs(y - height)) / Math.min(Math.abs(x), Math.abs(x - width));
                    var _a = findCorner(width, height, x, y, true), cx = _a[0], cy = _a[1];
                    rx = distance(cx - x, (cy - y) / c);
                    ry = c * rx;
                }
                break;
            case 1 /* FARTHEST_SIDE */:
                // Same as closest-side, except the ending shape is sized based on the farthest side(s)
                if (gradient.shape === 0 /* CIRCLE */) {
                    rx = ry = Math.max(Math.abs(x), Math.abs(x - width), Math.abs(y), Math.abs(y - height));
                }
                else if (gradient.shape === 1 /* ELLIPSE */) {
                    rx = Math.max(Math.abs(x), Math.abs(x - width));
                    ry = Math.max(Math.abs(y), Math.abs(y - height));
                }
                break;
            case 3 /* FARTHEST_CORNER */:
                // Same as closest-corner, except the ending shape is sized based on the farthest corner.
                // If the shape is an ellipse, the ending shape is given the same aspect ratio it would have if farthest-side were specified.
                if (gradient.shape === 0 /* CIRCLE */) {
                    rx = ry = Math.max(distance(x, y), distance(x, y - height), distance(x - width, y), distance(x - width, y - height));
                }
                else if (gradient.shape === 1 /* ELLIPSE */) {
                    // Compute the ratio ry/rx (which is to be the same as for "farthest-side")
                    var c = Math.max(Math.abs(y), Math.abs(y - height)) / Math.max(Math.abs(x), Math.abs(x - width));
                    var _b = findCorner(width, height, x, y, false), cx = _b[0], cy = _b[1];
                    rx = distance(cx - x, (cy - y) / c);
                    ry = c * rx;
                }
                break;
        }
        if (Array.isArray(gradient.size)) {
            rx = getAbsoluteValue(gradient.size[0], width);
            ry = gradient.size.length === 2 ? getAbsoluteValue(gradient.size[1], height) : rx;
        }
        return [rx, ry];
    };

    var linearGradient = function (context, tokens) {
        var angle$1 = deg(180);
        var stops = [];
        parseFunctionArgs(tokens).forEach(function (arg, i) {
            if (i === 0) {
                var firstToken = arg[0];
                if (firstToken.type === 20 /* IDENT_TOKEN */ && firstToken.value === 'to') {
                    angle$1 = parseNamedSide(arg);
                    return;
                }
                else if (isAngle(firstToken)) {
                    angle$1 = angle.parse(context, firstToken);
                    return;
                }
            }
            var colorStop = parseColorStop(context, arg);
            stops.push(colorStop);
        });
        return { angle: angle$1, stops: stops, type: 1 /* LINEAR_GRADIENT */ };
    };

    var prefixLinearGradient = function (context, tokens) {
        var angle$1 = deg(180);
        var stops = [];
        parseFunctionArgs(tokens).forEach(function (arg, i) {
            if (i === 0) {
                var firstToken = arg[0];
                if (firstToken.type === 20 /* IDENT_TOKEN */ &&
                    ['top', 'left', 'right', 'bottom'].indexOf(firstToken.value) !== -1) {
                    angle$1 = parseNamedSide(arg);
                    return;
                }
                else if (isAngle(firstToken)) {
                    angle$1 = (angle.parse(context, firstToken) + deg(270)) % deg(360);
                    return;
                }
            }
            var colorStop = parseColorStop(context, arg);
            stops.push(colorStop);
        });
        return {
            angle: angle$1,
            stops: stops,
            type: 1 /* LINEAR_GRADIENT */
        };
    };

    var webkitGradient = function (context, tokens) {
        var angle = deg(180);
        var stops = [];
        var type = 1 /* LINEAR_GRADIENT */;
        var shape = 0 /* CIRCLE */;
        var size = 3 /* FARTHEST_CORNER */;
        var position = [];
        parseFunctionArgs(tokens).forEach(function (arg, i) {
            var firstToken = arg[0];
            if (i === 0) {
                if (isIdentToken(firstToken) && firstToken.value === 'linear') {
                    type = 1 /* LINEAR_GRADIENT */;
                    return;
                }
                else if (isIdentToken(firstToken) && firstToken.value === 'radial') {
                    type = 2 /* RADIAL_GRADIENT */;
                    return;
                }
            }
            if (firstToken.type === 18 /* FUNCTION */) {
                if (firstToken.name === 'from') {
                    var color = color$1.parse(context, firstToken.values[0]);
                    stops.push({ stop: ZERO_LENGTH, color: color });
                }
                else if (firstToken.name === 'to') {
                    var color = color$1.parse(context, firstToken.values[0]);
                    stops.push({ stop: HUNDRED_PERCENT, color: color });
                }
                else if (firstToken.name === 'color-stop') {
                    var values = firstToken.values.filter(nonFunctionArgSeparator);
                    if (values.length === 2) {
                        var color = color$1.parse(context, values[1]);
                        var stop_1 = values[0];
                        if (isNumberToken(stop_1)) {
                            stops.push({
                                stop: { type: 16 /* PERCENTAGE_TOKEN */, number: stop_1.number * 100, flags: stop_1.flags },
                                color: color
                            });
                        }
                    }
                }
            }
        });
        return type === 1 /* LINEAR_GRADIENT */
            ? {
                angle: (angle + deg(180)) % deg(360),
                stops: stops,
                type: type
            }
            : { size: size, shape: shape, stops: stops, position: position, type: type };
    };

    var CLOSEST_SIDE = 'closest-side';
    var FARTHEST_SIDE = 'farthest-side';
    var CLOSEST_CORNER = 'closest-corner';
    var FARTHEST_CORNER = 'farthest-corner';
    var CIRCLE = 'circle';
    var ELLIPSE = 'ellipse';
    var COVER = 'cover';
    var CONTAIN = 'contain';
    var radialGradient = function (context, tokens) {
        var shape = 0 /* CIRCLE */;
        var size = 3 /* FARTHEST_CORNER */;
        var stops = [];
        var position = [];
        parseFunctionArgs(tokens).forEach(function (arg, i) {
            var isColorStop = true;
            if (i === 0) {
                var isAtPosition_1 = false;
                isColorStop = arg.reduce(function (acc, token) {
                    if (isAtPosition_1) {
                        if (isIdentToken(token)) {
                            switch (token.value) {
                                case 'center':
                                    position.push(FIFTY_PERCENT);
                                    return acc;
                                case 'top':
                                case 'left':
                                    position.push(ZERO_LENGTH);
                                    return acc;
                                case 'right':
                                case 'bottom':
                                    position.push(HUNDRED_PERCENT);
                                    return acc;
                            }
                        }
                        else if (isLengthPercentage(token) || isLength(token)) {
                            position.push(token);
                        }
                    }
                    else if (isIdentToken(token)) {
                        switch (token.value) {
                            case CIRCLE:
                                shape = 0 /* CIRCLE */;
                                return false;
                            case ELLIPSE:
                                shape = 1 /* ELLIPSE */;
                                return false;
                            case 'at':
                                isAtPosition_1 = true;
                                return false;
                            case CLOSEST_SIDE:
                                size = 0 /* CLOSEST_SIDE */;
                                return false;
                            case COVER:
                            case FARTHEST_SIDE:
                                size = 1 /* FARTHEST_SIDE */;
                                return false;
                            case CONTAIN:
                            case CLOSEST_CORNER:
                                size = 2 /* CLOSEST_CORNER */;
                                return false;
                            case FARTHEST_CORNER:
                                size = 3 /* FARTHEST_CORNER */;
                                return false;
                        }
                    }
                    else if (isLength(token) || isLengthPercentage(token)) {
                        if (!Array.isArray(size)) {
                            size = [];
                        }
                        size.push(token);
                        return false;
                    }
                    return acc;
                }, isColorStop);
            }
            if (isColorStop) {
                var colorStop = parseColorStop(context, arg);
                stops.push(colorStop);
            }
        });
        return { size: size, shape: shape, stops: stops, position: position, type: 2 /* RADIAL_GRADIENT */ };
    };

    var prefixRadialGradient = function (context, tokens) {
        var shape = 0 /* CIRCLE */;
        var size = 3 /* FARTHEST_CORNER */;
        var stops = [];
        var position = [];
        parseFunctionArgs(tokens).forEach(function (arg, i) {
            var isColorStop = true;
            if (i === 0) {
                isColorStop = arg.reduce(function (acc, token) {
                    if (isIdentToken(token)) {
                        switch (token.value) {
                            case 'center':
                                position.push(FIFTY_PERCENT);
                                return false;
                            case 'top':
                            case 'left':
                                position.push(ZERO_LENGTH);
                                return false;
                            case 'right':
                            case 'bottom':
                                position.push(HUNDRED_PERCENT);
                                return false;
                        }
                    }
                    else if (isLengthPercentage(token) || isLength(token)) {
                        position.push(token);
                        return false;
                    }
                    return acc;
                }, isColorStop);
            }
            else if (i === 1) {
                isColorStop = arg.reduce(function (acc, token) {
                    if (isIdentToken(token)) {
                        switch (token.value) {
                            case CIRCLE:
                                shape = 0 /* CIRCLE */;
                                return false;
                            case ELLIPSE:
                                shape = 1 /* ELLIPSE */;
                                return false;
                            case CONTAIN:
                            case CLOSEST_SIDE:
                                size = 0 /* CLOSEST_SIDE */;
                                return false;
                            case FARTHEST_SIDE:
                                size = 1 /* FARTHEST_SIDE */;
                                return false;
                            case CLOSEST_CORNER:
                                size = 2 /* CLOSEST_CORNER */;
                                return false;
                            case COVER:
                            case FARTHEST_CORNER:
                                size = 3 /* FARTHEST_CORNER */;
                                return false;
                        }
                    }
                    else if (isLength(token) || isLengthPercentage(token)) {
                        if (!Array.isArray(size)) {
                            size = [];
                        }
                        size.push(token);
                        return false;
                    }
                    return acc;
                }, isColorStop);
            }
            if (isColorStop) {
                var colorStop = parseColorStop(context, arg);
                stops.push(colorStop);
            }
        });
        return { size: size, shape: shape, stops: stops, position: position, type: 2 /* RADIAL_GRADIENT */ };
    };

    var isLinearGradient = function (background) {
        return background.type === 1 /* LINEAR_GRADIENT */;
    };
    var isRadialGradient = function (background) {
        return background.type === 2 /* RADIAL_GRADIENT */;
    };
    var image = {
        name: 'image',
        parse: function (context, value) {
            if (value.type === 22 /* URL_TOKEN */) {
                var image_1 = { url: value.value, type: 0 /* URL */ };
                context.cache.addImage(value.value);
                return image_1;
            }
            if (value.type === 18 /* FUNCTION */) {
                var imageFunction = SUPPORTED_IMAGE_FUNCTIONS[value.name];
                if (typeof imageFunction === 'undefined') {
                    throw new Error("Attempting to parse an unsupported image function \"" + value.name + "\"");
                }
                return imageFunction(context, value.values);
            }
            throw new Error("Unsupported image type " + value.type);
        }
    };
    function isSupportedImage(value) {
        return (!(value.type === 20 /* IDENT_TOKEN */ && value.value === 'none') &&
            (value.type !== 18 /* FUNCTION */ || !!SUPPORTED_IMAGE_FUNCTIONS[value.name]));
    }
    var SUPPORTED_IMAGE_FUNCTIONS = {
        'linear-gradient': linearGradient,
        '-moz-linear-gradient': prefixLinearGradient,
        '-ms-linear-gradient': prefixLinearGradient,
        '-o-linear-gradient': prefixLinearGradient,
        '-webkit-linear-gradient': prefixLinearGradient,
        'radial-gradient': radialGradient,
        '-moz-radial-gradient': prefixRadialGradient,
        '-ms-radial-gradient': prefixRadialGradient,
        '-o-radial-gradient': prefixRadialGradient,
        '-webkit-radial-gradient': prefixRadialGradient,
        '-webkit-gradient': webkitGradient
    };

    var backgroundImage = {
        name: 'background-image',
        initialValue: 'none',
        type: 1 /* LIST */,
        prefix: false,
        parse: function (context, tokens) {
            if (tokens.length === 0) {
                return [];
            }
            var first = tokens[0];
            if (first.type === 20 /* IDENT_TOKEN */ && first.value === 'none') {
                return [];
            }
            return tokens
                .filter(function (value) { return nonFunctionArgSeparator(value) && isSupportedImage(value); })
                .map(function (value) { return image.parse(context, value); });
        }
    };

    var backgroundOrigin = {
        name: 'background-origin',
        initialValue: 'border-box',
        prefix: false,
        type: 1 /* LIST */,
        parse: function (_context, tokens) {
            return tokens.map(function (token) {
                if (isIdentToken(token)) {
                    switch (token.value) {
                        case 'padding-box':
                            return 1 /* PADDING_BOX */;
                        case 'content-box':
                            return 2 /* CONTENT_BOX */;
                    }
                }
                return 0 /* BORDER_BOX */;
            });
        }
    };

    var backgroundPosition = {
        name: 'background-position',
        initialValue: '0% 0%',
        type: 1 /* LIST */,
        prefix: false,
        parse: function (_context, tokens) {
            return parseFunctionArgs(tokens)
                .map(function (values) { return values.filter(isLengthPercentage); })
                .map(parseLengthPercentageTuple);
        }
    };

    var backgroundRepeat = {
        name: 'background-repeat',
        initialValue: 'repeat',
        prefix: false,
        type: 1 /* LIST */,
        parse: function (_context, tokens) {
            return parseFunctionArgs(tokens)
                .map(function (values) {
                return values
                    .filter(isIdentToken)
                    .map(function (token) { return token.value; })
                    .join(' ');
            })
                .map(parseBackgroundRepeat);
        }
    };
    var parseBackgroundRepeat = function (value) {
        switch (value) {
            case 'no-repeat':
                return 1 /* NO_REPEAT */;
            case 'repeat-x':
            case 'repeat no-repeat':
                return 2 /* REPEAT_X */;
            case 'repeat-y':
            case 'no-repeat repeat':
                return 3 /* REPEAT_Y */;
            case 'repeat':
            default:
                return 0 /* REPEAT */;
        }
    };

    var BACKGROUND_SIZE;
    (function (BACKGROUND_SIZE) {
        BACKGROUND_SIZE["AUTO"] = "auto";
        BACKGROUND_SIZE["CONTAIN"] = "contain";
        BACKGROUND_SIZE["COVER"] = "cover";
    })(BACKGROUND_SIZE || (BACKGROUND_SIZE = {}));
    var backgroundSize = {
        name: 'background-size',
        initialValue: '0',
        prefix: false,
        type: 1 /* LIST */,
        parse: function (_context, tokens) {
            return parseFunctionArgs(tokens).map(function (values) { return values.filter(isBackgroundSizeInfoToken); });
        }
    };
    var isBackgroundSizeInfoToken = function (value) {
        return isIdentToken(value) || isLengthPercentage(value);
    };

    var borderColorForSide = function (side) { return ({
        name: "border-" + side + "-color",
        initialValue: 'transparent',
        prefix: false,
        type: 3 /* TYPE_VALUE */,
        format: 'color'
    }); };
    var borderTopColor = borderColorForSide('top');
    var borderRightColor = borderColorForSide('right');
    var borderBottomColor = borderColorForSide('bottom');
    var borderLeftColor = borderColorForSide('left');

    var borderRadiusForSide = function (side) { return ({
        name: "border-radius-" + side,
        initialValue: '0 0',
        prefix: false,
        type: 1 /* LIST */,
        parse: function (_context, tokens) {
            return parseLengthPercentageTuple(tokens.filter(isLengthPercentage));
        }
    }); };
    var borderTopLeftRadius = borderRadiusForSide('top-left');
    var borderTopRightRadius = borderRadiusForSide('top-right');
    var borderBottomRightRadius = borderRadiusForSide('bottom-right');
    var borderBottomLeftRadius = borderRadiusForSide('bottom-left');

    var borderStyleForSide = function (side) { return ({
        name: "border-" + side + "-style",
        initialValue: 'solid',
        prefix: false,
        type: 2 /* IDENT_VALUE */,
        parse: function (_context, style) {
            switch (style) {
                case 'none':
                    return 0 /* NONE */;
                case 'dashed':
                    return 2 /* DASHED */;
                case 'dotted':
                    return 3 /* DOTTED */;
                case 'double':
                    return 4 /* DOUBLE */;
            }
            return 1 /* SOLID */;
        }
    }); };
    var borderTopStyle = borderStyleForSide('top');
    var borderRightStyle = borderStyleForSide('right');
    var borderBottomStyle = borderStyleForSide('bottom');
    var borderLeftStyle = borderStyleForSide('left');

    var borderWidthForSide = function (side) { return ({
        name: "border-" + side + "-width",
        initialValue: '0',
        type: 0 /* VALUE */,
        prefix: false,
        parse: function (_context, token) {
            if (isDimensionToken(token)) {
                return token.number;
            }
            return 0;
        }
    }); };
    var borderTopWidth = borderWidthForSide('top');
    var borderRightWidth = borderWidthForSide('right');
    var borderBottomWidth = borderWidthForSide('bottom');
    var borderLeftWidth = borderWidthForSide('left');

    var color = {
        name: "color",
        initialValue: 'transparent',
        prefix: false,
        type: 3 /* TYPE_VALUE */,
        format: 'color'
    };

    var direction = {
        name: 'direction',
        initialValue: 'ltr',
        prefix: false,
        type: 2 /* IDENT_VALUE */,
        parse: function (_context, direction) {
            switch (direction) {
                case 'rtl':
                    return 1 /* RTL */;
                case 'ltr':
                default:
                    return 0 /* LTR */;
            }
        }
    };

    var display = {
        name: 'display',
        initialValue: 'inline-block',
        prefix: false,
        type: 1 /* LIST */,
        parse: function (_context, tokens) {
            return tokens.filter(isIdentToken).reduce(function (bit, token) {
                return bit | parseDisplayValue(token.value);
            }, 0 /* NONE */);
        }
    };
    var parseDisplayValue = function (display) {
        switch (display) {
            case 'block':
            case '-webkit-box':
                return 2 /* BLOCK */;
            case 'inline':
                return 4 /* INLINE */;
            case 'run-in':
                return 8 /* RUN_IN */;
            case 'flow':
                return 16 /* FLOW */;
            case 'flow-root':
                return 32 /* FLOW_ROOT */;
            case 'table':
                return 64 /* TABLE */;
            case 'flex':
            case '-webkit-flex':
                return 128 /* FLEX */;
            case 'grid':
            case '-ms-grid':
                return 256 /* GRID */;
            case 'ruby':
                return 512 /* RUBY */;
            case 'subgrid':
                return 1024 /* SUBGRID */;
            case 'list-item':
                return 2048 /* LIST_ITEM */;
            case 'table-row-group':
                return 4096 /* TABLE_ROW_GROUP */;
            case 'table-header-group':
                return 8192 /* TABLE_HEADER_GROUP */;
            case 'table-footer-group':
                return 16384 /* TABLE_FOOTER_GROUP */;
            case 'table-row':
                return 32768 /* TABLE_ROW */;
            case 'table-cell':
                return 65536 /* TABLE_CELL */;
            case 'table-column-group':
                return 131072 /* TABLE_COLUMN_GROUP */;
            case 'table-column':
                return 262144 /* TABLE_COLUMN */;
            case 'table-caption':
                return 524288 /* TABLE_CAPTION */;
            case 'ruby-base':
                return 1048576 /* RUBY_BASE */;
            case 'ruby-text':
                return 2097152 /* RUBY_TEXT */;
            case 'ruby-base-container':
                return 4194304 /* RUBY_BASE_CONTAINER */;
            case 'ruby-text-container':
                return 8388608 /* RUBY_TEXT_CONTAINER */;
            case 'contents':
                return 16777216 /* CONTENTS */;
            case 'inline-block':
                return 33554432 /* INLINE_BLOCK */;
            case 'inline-list-item':
                return 67108864 /* INLINE_LIST_ITEM */;
            case 'inline-table':
                return 134217728 /* INLINE_TABLE */;
            case 'inline-flex':
                return 268435456 /* INLINE_FLEX */;
            case 'inline-grid':
                return 536870912 /* INLINE_GRID */;
        }
        return 0 /* NONE */;
    };

    var float = {
        name: 'float',
        initialValue: 'none',
        prefix: false,
        type: 2 /* IDENT_VALUE */,
        parse: function (_context, float) {
            switch (float) {
                case 'left':
                    return 1 /* LEFT */;
                case 'right':
                    return 2 /* RIGHT */;
                case 'inline-start':
                    return 3 /* INLINE_START */;
                case 'inline-end':
                    return 4 /* INLINE_END */;
            }
            return 0 /* NONE */;
        }
    };

    var letterSpacing = {
        name: 'letter-spacing',
        initialValue: '0',
        prefix: false,
        type: 0 /* VALUE */,
        parse: function (_context, token) {
            if (token.type === 20 /* IDENT_TOKEN */ && token.value === 'normal') {
                return 0;
            }
            if (token.type === 17 /* NUMBER_TOKEN */) {
                return token.number;
            }
            if (token.type === 15 /* DIMENSION_TOKEN */) {
                return token.number;
            }
            return 0;
        }
    };

    var LINE_BREAK;
    (function (LINE_BREAK) {
        LINE_BREAK["NORMAL"] = "normal";
        LINE_BREAK["STRICT"] = "strict";
    })(LINE_BREAK || (LINE_BREAK = {}));
    var lineBreak = {
        name: 'line-break',
        initialValue: 'normal',
        prefix: false,
        type: 2 /* IDENT_VALUE */,
        parse: function (_context, lineBreak) {
            switch (lineBreak) {
                case 'strict':
                    return LINE_BREAK.STRICT;
                case 'normal':
                default:
                    return LINE_BREAK.NORMAL;
            }
        }
    };

    var lineHeight = {
        name: 'line-height',
        initialValue: 'normal',
        prefix: false,
        type: 4 /* TOKEN_VALUE */
    };
    var computeLineHeight = function (token, fontSize) {
        if (isIdentToken(token) && token.value === 'normal') {
            return 1.2 * fontSize;
        }
        else if (token.type === 17 /* NUMBER_TOKEN */) {
            return fontSize * token.number;
        }
        else if (isLengthPercentage(token)) {
            return getAbsoluteValue(token, fontSize);
        }
        return fontSize;
    };

    var listStyleImage = {
        name: 'list-style-image',
        initialValue: 'none',
        type: 0 /* VALUE */,
        prefix: false,
        parse: function (context, token) {
            if (token.type === 20 /* IDENT_TOKEN */ && token.value === 'none') {
                return null;
            }
            return image.parse(context, token);
        }
    };

    var listStylePosition = {
        name: 'list-style-position',
        initialValue: 'outside',
        prefix: false,
        type: 2 /* IDENT_VALUE */,
        parse: function (_context, position) {
            switch (position) {
                case 'inside':
                    return 0 /* INSIDE */;
                case 'outside':
                default:
                    return 1 /* OUTSIDE */;
            }
        }
    };

    var listStyleType = {
        name: 'list-style-type',
        initialValue: 'none',
        prefix: false,
        type: 2 /* IDENT_VALUE */,
        parse: function (_context, type) {
            switch (type) {
                case 'disc':
                    return 0 /* DISC */;
                case 'circle':
                    return 1 /* CIRCLE */;
                case 'square':
                    return 2 /* SQUARE */;
                case 'decimal':
                    return 3 /* DECIMAL */;
                case 'cjk-decimal':
                    return 4 /* CJK_DECIMAL */;
                case 'decimal-leading-zero':
                    return 5 /* DECIMAL_LEADING_ZERO */;
                case 'lower-roman':
                    return 6 /* LOWER_ROMAN */;
                case 'upper-roman':
                    return 7 /* UPPER_ROMAN */;
                case 'lower-greek':
                    return 8 /* LOWER_GREEK */;
                case 'lower-alpha':
                    return 9 /* LOWER_ALPHA */;
                case 'upper-alpha':
                    return 10 /* UPPER_ALPHA */;
                case 'arabic-indic':
                    return 11 /* ARABIC_INDIC */;
                case 'armenian':
                    return 12 /* ARMENIAN */;
                case 'bengali':
                    return 13 /* BENGALI */;
                case 'cambodian':
                    return 14 /* CAMBODIAN */;
                case 'cjk-earthly-branch':
                    return 15 /* CJK_EARTHLY_BRANCH */;
                case 'cjk-heavenly-stem':
                    return 16 /* CJK_HEAVENLY_STEM */;
                case 'cjk-ideographic':
                    return 17 /* CJK_IDEOGRAPHIC */;
                case 'devanagari':
                    return 18 /* DEVANAGARI */;
                case 'ethiopic-numeric':
                    return 19 /* ETHIOPIC_NUMERIC */;
                case 'georgian':
                    return 20 /* GEORGIAN */;
                case 'gujarati':
                    return 21 /* GUJARATI */;
                case 'gurmukhi':
                    return 22 /* GURMUKHI */;
                case 'hebrew':
                    return 22 /* HEBREW */;
                case 'hiragana':
                    return 23 /* HIRAGANA */;
                case 'hiragana-iroha':
                    return 24 /* HIRAGANA_IROHA */;
                case 'japanese-formal':
                    return 25 /* JAPANESE_FORMAL */;
                case 'japanese-informal':
                    return 26 /* JAPANESE_INFORMAL */;
                case 'kannada':
                    return 27 /* KANNADA */;
                case 'katakana':
                    return 28 /* KATAKANA */;
                case 'katakana-iroha':
                    return 29 /* KATAKANA_IROHA */;
                case 'khmer':
                    return 30 /* KHMER */;
                case 'korean-hangul-formal':
                    return 31 /* KOREAN_HANGUL_FORMAL */;
                case 'korean-hanja-formal':
                    return 32 /* KOREAN_HANJA_FORMAL */;
                case 'korean-hanja-informal':
                    return 33 /* KOREAN_HANJA_INFORMAL */;
                case 'lao':
                    return 34 /* LAO */;
                case 'lower-armenian':
                    return 35 /* LOWER_ARMENIAN */;
                case 'malayalam':
                    return 36 /* MALAYALAM */;
                case 'mongolian':
                    return 37 /* MONGOLIAN */;
                case 'myanmar':
                    return 38 /* MYANMAR */;
                case 'oriya':
                    return 39 /* ORIYA */;
                case 'persian':
                    return 40 /* PERSIAN */;
                case 'simp-chinese-formal':
                    return 41 /* SIMP_CHINESE_FORMAL */;
                case 'simp-chinese-informal':
                    return 42 /* SIMP_CHINESE_INFORMAL */;
                case 'tamil':
                    return 43 /* TAMIL */;
                case 'telugu':
                    return 44 /* TELUGU */;
                case 'thai':
                    return 45 /* THAI */;
                case 'tibetan':
                    return 46 /* TIBETAN */;
                case 'trad-chinese-formal':
                    return 47 /* TRAD_CHINESE_FORMAL */;
                case 'trad-chinese-informal':
                    return 48 /* TRAD_CHINESE_INFORMAL */;
                case 'upper-armenian':
                    return 49 /* UPPER_ARMENIAN */;
                case 'disclosure-open':
                    return 50 /* DISCLOSURE_OPEN */;
                case 'disclosure-closed':
                    return 51 /* DISCLOSURE_CLOSED */;
                case 'none':
                default:
                    return -1 /* NONE */;
            }
        }
    };

    var marginForSide = function (side) { return ({
        name: "margin-" + side,
        initialValue: '0',
        prefix: false,
        type: 4 /* TOKEN_VALUE */
    }); };
    var marginTop = marginForSide('top');
    var marginRight = marginForSide('right');
    var marginBottom = marginForSide('bottom');
    var marginLeft = marginForSide('left');

    var overflow = {
        name: 'overflow',
        initialValue: 'visible',
        prefix: false,
        type: 1 /* LIST */,
        parse: function (_context, tokens) {
            return tokens.filter(isIdentToken).map(function (overflow) {
                switch (overflow.value) {
                    case 'hidden':
                        return 1 /* HIDDEN */;
                    case 'scroll':
                        return 2 /* SCROLL */;
                    case 'clip':
                        return 3 /* CLIP */;
                    case 'auto':
                        return 4 /* AUTO */;
                    case 'visible':
                    default:
                        return 0 /* VISIBLE */;
                }
            });
        }
    };

    var overflowWrap = {
        name: 'overflow-wrap',
        initialValue: 'normal',
        prefix: false,
        type: 2 /* IDENT_VALUE */,
        parse: function (_context, overflow) {
            switch (overflow) {
                case 'break-word':
                    return "break-word" /* BREAK_WORD */;
                case 'normal':
                default:
                    return "normal" /* NORMAL */;
            }
        }
    };

    var paddingForSide = function (side) { return ({
        name: "padding-" + side,
        initialValue: '0',
        prefix: false,
        type: 3 /* TYPE_VALUE */,
        format: 'length-percentage'
    }); };
    var paddingTop = paddingForSide('top');
    var paddingRight = paddingForSide('right');
    var paddingBottom = paddingForSide('bottom');
    var paddingLeft = paddingForSide('left');

    var textAlign = {
        name: 'text-align',
        initialValue: 'left',
        prefix: false,
        type: 2 /* IDENT_VALUE */,
        parse: function (_context, textAlign) {
            switch (textAlign) {
                case 'right':
                    return 2 /* RIGHT */;
                case 'center':
                case 'justify':
                    return 1 /* CENTER */;
                case 'left':
                default:
                    return 0 /* LEFT */;
            }
        }
    };

    var position = {
        name: 'position',
        initialValue: 'static',
        prefix: false,
        type: 2 /* IDENT_VALUE */,
        parse: function (_context, position) {
            switch (position) {
                case 'relative':
                    return 1 /* RELATIVE */;
                case 'absolute':
                    return 2 /* ABSOLUTE */;
                case 'fixed':
                    return 3 /* FIXED */;
                case 'sticky':
                    return 4 /* STICKY */;
            }
            return 0 /* STATIC */;
        }
    };

    var textShadow = {
        name: 'text-shadow',
        initialValue: 'none',
        type: 1 /* LIST */,
        prefix: false,
        parse: function (context, tokens) {
            if (tokens.length === 1 && isIdentWithValue(tokens[0], 'none')) {
                return [];
            }
            return parseFunctionArgs(tokens).map(function (values) {
                var shadow = {
                    color: COLORS.TRANSPARENT,
                    offsetX: ZERO_LENGTH,
                    offsetY: ZERO_LENGTH,
                    blur: ZERO_LENGTH
                };
                var c = 0;
                for (var i = 0; i < values.length; i++) {
                    var token = values[i];
                    if (isLength(token)) {
                        if (c === 0) {
                            shadow.offsetX = token;
                        }
                        else if (c === 1) {
                            shadow.offsetY = token;
                        }
                        else {
                            shadow.blur = token;
                        }
                        c++;
                    }
                    else {
                        shadow.color = color$1.parse(context, token);
                    }
                }
                return shadow;
            });
        }
    };

    var textTransform = {
        name: 'text-transform',
        initialValue: 'none',
        prefix: false,
        type: 2 /* IDENT_VALUE */,
        parse: function (_context, textTransform) {
            switch (textTransform) {
                case 'uppercase':
                    return 2 /* UPPERCASE */;
                case 'lowercase':
                    return 1 /* LOWERCASE */;
                case 'capitalize':
                    return 3 /* CAPITALIZE */;
            }
            return 0 /* NONE */;
        }
    };

    var transform$1 = {
        name: 'transform',
        initialValue: 'none',
        prefix: true,
        type: 0 /* VALUE */,
        parse: function (_context, token) {
            if (token.type === 20 /* IDENT_TOKEN */ && token.value === 'none') {
                return null;
            }
            if (token.type === 18 /* FUNCTION */) {
                var transformFunction = SUPPORTED_TRANSFORM_FUNCTIONS[token.name];
                if (typeof transformFunction === 'undefined') {
                    throw new Error("Attempting to parse an unsupported transform function \"" + token.name + "\"");
                }
                return transformFunction(token.values);
            }
            return null;
        }
    };
    var matrix = function (args) {
        var values = args.filter(function (arg) { return arg.type === 17 /* NUMBER_TOKEN */; }).map(function (arg) { return arg.number; });
        return values.length === 6 ? values : null;
    };
    // doesn't support 3D transforms at the moment
    var matrix3d = function (args) {
        var values = args.filter(function (arg) { return arg.type === 17 /* NUMBER_TOKEN */; }).map(function (arg) { return arg.number; });
        var a1 = values[0], b1 = values[1]; values[2]; values[3]; var a2 = values[4], b2 = values[5]; values[6]; values[7]; values[8]; values[9]; values[10]; values[11]; var a4 = values[12], b4 = values[13]; values[14]; values[15];
        return values.length === 16 ? [a1, b1, a2, b2, a4, b4] : null;
    };
    var SUPPORTED_TRANSFORM_FUNCTIONS = {
        matrix: matrix,
        matrix3d: matrix3d
    };

    var DEFAULT_VALUE = {
        type: 16 /* PERCENTAGE_TOKEN */,
        number: 50,
        flags: FLAG_INTEGER
    };
    var DEFAULT = [DEFAULT_VALUE, DEFAULT_VALUE];
    var transformOrigin = {
        name: 'transform-origin',
        initialValue: '50% 50%',
        prefix: true,
        type: 1 /* LIST */,
        parse: function (_context, tokens) {
            var origins = tokens.filter(isLengthPercentage);
            if (origins.length !== 2) {
                return DEFAULT;
            }
            return [origins[0], origins[1]];
        }
    };

    var visibility = {
        name: 'visible',
        initialValue: 'none',
        prefix: false,
        type: 2 /* IDENT_VALUE */,
        parse: function (_context, visibility) {
            switch (visibility) {
                case 'hidden':
                    return 1 /* HIDDEN */;
                case 'collapse':
                    return 2 /* COLLAPSE */;
                case 'visible':
                default:
                    return 0 /* VISIBLE */;
            }
        }
    };

    var WORD_BREAK;
    (function (WORD_BREAK) {
        WORD_BREAK["NORMAL"] = "normal";
        WORD_BREAK["BREAK_ALL"] = "break-all";
        WORD_BREAK["KEEP_ALL"] = "keep-all";
    })(WORD_BREAK || (WORD_BREAK = {}));
    var wordBreak = {
        name: 'word-break',
        initialValue: 'normal',
        prefix: false,
        type: 2 /* IDENT_VALUE */,
        parse: function (_context, wordBreak) {
            switch (wordBreak) {
                case 'break-all':
                    return WORD_BREAK.BREAK_ALL;
                case 'keep-all':
                    return WORD_BREAK.KEEP_ALL;
                case 'normal':
                default:
                    return WORD_BREAK.NORMAL;
            }
        }
    };

    var zIndex = {
        name: 'z-index',
        initialValue: 'auto',
        prefix: false,
        type: 0 /* VALUE */,
        parse: function (_context, token) {
            if (token.type === 20 /* IDENT_TOKEN */) {
                return { auto: true, order: 0 };
            }
            if (isNumberToken(token)) {
                return { auto: false, order: token.number };
            }
            throw new Error("Invalid z-index number parsed");
        }
    };

    var time = {
        name: 'time',
        parse: function (_context, value) {
            if (value.type === 15 /* DIMENSION_TOKEN */) {
                switch (value.unit.toLowerCase()) {
                    case 's':
                        return 1000 * value.number;
                    case 'ms':
                        return value.number;
                }
            }
            throw new Error("Unsupported time type");
        }
    };

    var opacity = {
        name: 'opacity',
        initialValue: '1',
        type: 0 /* VALUE */,
        prefix: false,
        parse: function (_context, token) {
            if (isNumberToken(token)) {
                return token.number;
            }
            return 1;
        }
    };

    var textDecorationColor = {
        name: "text-decoration-color",
        initialValue: 'transparent',
        prefix: false,
        type: 3 /* TYPE_VALUE */,
        format: 'color'
    };

    var textDecorationLine = {
        name: 'text-decoration-line',
        initialValue: 'none',
        prefix: false,
        type: 1 /* LIST */,
        parse: function (_context, tokens) {
            return tokens
                .filter(isIdentToken)
                .map(function (token) {
                switch (token.value) {
                    case 'underline':
                        return 1 /* UNDERLINE */;
                    case 'overline':
                        return 2 /* OVERLINE */;
                    case 'line-through':
                        return 3 /* LINE_THROUGH */;
                    case 'none':
                        return 4 /* BLINK */;
                }
                return 0 /* NONE */;
            })
                .filter(function (line) { return line !== 0 /* NONE */; });
        }
    };

    var fontFamily = {
        name: "font-family",
        initialValue: '',
        prefix: false,
        type: 1 /* LIST */,
        parse: function (_context, tokens) {
            var accumulator = [];
            var results = [];
            tokens.forEach(function (token) {
                switch (token.type) {
                    case 20 /* IDENT_TOKEN */:
                    case 0 /* STRING_TOKEN */:
                        accumulator.push(token.value);
                        break;
                    case 17 /* NUMBER_TOKEN */:
                        accumulator.push(token.number.toString());
                        break;
                    case 4 /* COMMA_TOKEN */:
                        results.push(accumulator.join(' '));
                        accumulator.length = 0;
                        break;
                }
            });
            if (accumulator.length) {
                results.push(accumulator.join(' '));
            }
            return results.map(function (result) { return (result.indexOf(' ') === -1 ? result : "'" + result + "'"); });
        }
    };

    var fontSize = {
        name: "font-size",
        initialValue: '0',
        prefix: false,
        type: 3 /* TYPE_VALUE */,
        format: 'length'
    };

    var fontWeight = {
        name: 'font-weight',
        initialValue: 'normal',
        type: 0 /* VALUE */,
        prefix: false,
        parse: function (_context, token) {
            if (isNumberToken(token)) {
                return token.number;
            }
            if (isIdentToken(token)) {
                switch (token.value) {
                    case 'bold':
                        return 700;
                    case 'normal':
                    default:
                        return 400;
                }
            }
            return 400;
        }
    };

    var fontVariant = {
        name: 'font-variant',
        initialValue: 'none',
        type: 1 /* LIST */,
        prefix: false,
        parse: function (_context, tokens) {
            return tokens.filter(isIdentToken).map(function (token) { return token.value; });
        }
    };

    var fontStyle = {
        name: 'font-style',
        initialValue: 'normal',
        prefix: false,
        type: 2 /* IDENT_VALUE */,
        parse: function (_context, overflow) {
            switch (overflow) {
                case 'oblique':
                    return "oblique" /* OBLIQUE */;
                case 'italic':
                    return "italic" /* ITALIC */;
                case 'normal':
                default:
                    return "normal" /* NORMAL */;
            }
        }
    };

    var contains = function (bit, value) { return (bit & value) !== 0; };

    var content = {
        name: 'content',
        initialValue: 'none',
        type: 1 /* LIST */,
        prefix: false,
        parse: function (_context, tokens) {
            if (tokens.length === 0) {
                return [];
            }
            var first = tokens[0];
            if (first.type === 20 /* IDENT_TOKEN */ && first.value === 'none') {
                return [];
            }
            return tokens;
        }
    };

    var counterIncrement = {
        name: 'counter-increment',
        initialValue: 'none',
        prefix: true,
        type: 1 /* LIST */,
        parse: function (_context, tokens) {
            if (tokens.length === 0) {
                return null;
            }
            var first = tokens[0];
            if (first.type === 20 /* IDENT_TOKEN */ && first.value === 'none') {
                return null;
            }
            var increments = [];
            var filtered = tokens.filter(nonWhiteSpace);
            for (var i = 0; i < filtered.length; i++) {
                var counter = filtered[i];
                var next = filtered[i + 1];
                if (counter.type === 20 /* IDENT_TOKEN */) {
                    var increment = next && isNumberToken(next) ? next.number : 1;
                    increments.push({ counter: counter.value, increment: increment });
                }
            }
            return increments;
        }
    };

    var counterReset = {
        name: 'counter-reset',
        initialValue: 'none',
        prefix: true,
        type: 1 /* LIST */,
        parse: function (_context, tokens) {
            if (tokens.length === 0) {
                return [];
            }
            var resets = [];
            var filtered = tokens.filter(nonWhiteSpace);
            for (var i = 0; i < filtered.length; i++) {
                var counter = filtered[i];
                var next = filtered[i + 1];
                if (isIdentToken(counter) && counter.value !== 'none') {
                    var reset = next && isNumberToken(next) ? next.number : 0;
                    resets.push({ counter: counter.value, reset: reset });
                }
            }
            return resets;
        }
    };

    var duration = {
        name: 'duration',
        initialValue: '0s',
        prefix: false,
        type: 1 /* LIST */,
        parse: function (context, tokens) {
            return tokens.filter(isDimensionToken).map(function (token) { return time.parse(context, token); });
        }
    };

    var quotes = {
        name: 'quotes',
        initialValue: 'none',
        prefix: true,
        type: 1 /* LIST */,
        parse: function (_context, tokens) {
            if (tokens.length === 0) {
                return null;
            }
            var first = tokens[0];
            if (first.type === 20 /* IDENT_TOKEN */ && first.value === 'none') {
                return null;
            }
            var quotes = [];
            var filtered = tokens.filter(isStringToken);
            if (filtered.length % 2 !== 0) {
                return null;
            }
            for (var i = 0; i < filtered.length; i += 2) {
                var open_1 = filtered[i].value;
                var close_1 = filtered[i + 1].value;
                quotes.push({ open: open_1, close: close_1 });
            }
            return quotes;
        }
    };
    var getQuote = function (quotes, depth, open) {
        if (!quotes) {
            return '';
        }
        var quote = quotes[Math.min(depth, quotes.length - 1)];
        if (!quote) {
            return '';
        }
        return open ? quote.open : quote.close;
    };

    var boxShadow = {
        name: 'box-shadow',
        initialValue: 'none',
        type: 1 /* LIST */,
        prefix: false,
        parse: function (context, tokens) {
            if (tokens.length === 1 && isIdentWithValue(tokens[0], 'none')) {
                return [];
            }
            return parseFunctionArgs(tokens).map(function (values) {
                var shadow = {
                    color: 0x000000ff,
                    offsetX: ZERO_LENGTH,
                    offsetY: ZERO_LENGTH,
                    blur: ZERO_LENGTH,
                    spread: ZERO_LENGTH,
                    inset: false
                };
                var c = 0;
                for (var i = 0; i < values.length; i++) {
                    var token = values[i];
                    if (isIdentWithValue(token, 'inset')) {
                        shadow.inset = true;
                    }
                    else if (isLength(token)) {
                        if (c === 0) {
                            shadow.offsetX = token;
                        }
                        else if (c === 1) {
                            shadow.offsetY = token;
                        }
                        else if (c === 2) {
                            shadow.blur = token;
                        }
                        else {
                            shadow.spread = token;
                        }
                        c++;
                    }
                    else {
                        shadow.color = color$1.parse(context, token);
                    }
                }
                return shadow;
            });
        }
    };

    var paintOrder = {
        name: 'paint-order',
        initialValue: 'normal',
        prefix: false,
        type: 1 /* LIST */,
        parse: function (_context, tokens) {
            var DEFAULT_VALUE = [0 /* FILL */, 1 /* STROKE */, 2 /* MARKERS */];
            var layers = [];
            tokens.filter(isIdentToken).forEach(function (token) {
                switch (token.value) {
                    case 'stroke':
                        layers.push(1 /* STROKE */);
                        break;
                    case 'fill':
                        layers.push(0 /* FILL */);
                        break;
                    case 'markers':
                        layers.push(2 /* MARKERS */);
                        break;
                }
            });
            DEFAULT_VALUE.forEach(function (value) {
                if (layers.indexOf(value) === -1) {
                    layers.push(value);
                }
            });
            return layers;
        }
    };

    var webkitTextStrokeColor = {
        name: "-webkit-text-stroke-color",
        initialValue: 'currentcolor',
        prefix: false,
        type: 3 /* TYPE_VALUE */,
        format: 'color'
    };

    var webkitTextStrokeWidth = {
        name: "-webkit-text-stroke-width",
        initialValue: '0',
        type: 0 /* VALUE */,
        prefix: false,
        parse: function (_context, token) {
            if (isDimensionToken(token)) {
                return token.number;
            }
            return 0;
        }
    };

    var CSSParsedDeclaration = /** @class */ (function () {
        function CSSParsedDeclaration(context, declaration) {
            var _a, _b;
            this.animationDuration = parse(context, duration, declaration.animationDuration);
            this.backgroundClip = parse(context, backgroundClip, declaration.backgroundClip);
            this.backgroundColor = parse(context, backgroundColor, declaration.backgroundColor);
            this.backgroundImage = parse(context, backgroundImage, declaration.backgroundImage);
            this.backgroundOrigin = parse(context, backgroundOrigin, declaration.backgroundOrigin);
            this.backgroundPosition = parse(context, backgroundPosition, declaration.backgroundPosition);
            this.backgroundRepeat = parse(context, backgroundRepeat, declaration.backgroundRepeat);
            this.backgroundSize = parse(context, backgroundSize, declaration.backgroundSize);
            this.borderTopColor = parse(context, borderTopColor, declaration.borderTopColor);
            this.borderRightColor = parse(context, borderRightColor, declaration.borderRightColor);
            this.borderBottomColor = parse(context, borderBottomColor, declaration.borderBottomColor);
            this.borderLeftColor = parse(context, borderLeftColor, declaration.borderLeftColor);
            this.borderTopLeftRadius = parse(context, borderTopLeftRadius, declaration.borderTopLeftRadius);
            this.borderTopRightRadius = parse(context, borderTopRightRadius, declaration.borderTopRightRadius);
            this.borderBottomRightRadius = parse(context, borderBottomRightRadius, declaration.borderBottomRightRadius);
            this.borderBottomLeftRadius = parse(context, borderBottomLeftRadius, declaration.borderBottomLeftRadius);
            this.borderTopStyle = parse(context, borderTopStyle, declaration.borderTopStyle);
            this.borderRightStyle = parse(context, borderRightStyle, declaration.borderRightStyle);
            this.borderBottomStyle = parse(context, borderBottomStyle, declaration.borderBottomStyle);
            this.borderLeftStyle = parse(context, borderLeftStyle, declaration.borderLeftStyle);
            this.borderTopWidth = parse(context, borderTopWidth, declaration.borderTopWidth);
            this.borderRightWidth = parse(context, borderRightWidth, declaration.borderRightWidth);
            this.borderBottomWidth = parse(context, borderBottomWidth, declaration.borderBottomWidth);
            this.borderLeftWidth = parse(context, borderLeftWidth, declaration.borderLeftWidth);
            this.boxShadow = parse(context, boxShadow, declaration.boxShadow);
            this.color = parse(context, color, declaration.color);
            this.direction = parse(context, direction, declaration.direction);
            this.display = parse(context, display, declaration.display);
            this.float = parse(context, float, declaration.cssFloat);
            this.fontFamily = parse(context, fontFamily, declaration.fontFamily);
            this.fontSize = parse(context, fontSize, declaration.fontSize);
            this.fontStyle = parse(context, fontStyle, declaration.fontStyle);
            this.fontVariant = parse(context, fontVariant, declaration.fontVariant);
            this.fontWeight = parse(context, fontWeight, declaration.fontWeight);
            this.letterSpacing = parse(context, letterSpacing, declaration.letterSpacing);
            this.lineBreak = parse(context, lineBreak, declaration.lineBreak);
            this.lineHeight = parse(context, lineHeight, declaration.lineHeight);
            this.listStyleImage = parse(context, listStyleImage, declaration.listStyleImage);
            this.listStylePosition = parse(context, listStylePosition, declaration.listStylePosition);
            this.listStyleType = parse(context, listStyleType, declaration.listStyleType);
            this.marginTop = parse(context, marginTop, declaration.marginTop);
            this.marginRight = parse(context, marginRight, declaration.marginRight);
            this.marginBottom = parse(context, marginBottom, declaration.marginBottom);
            this.marginLeft = parse(context, marginLeft, declaration.marginLeft);
            this.opacity = parse(context, opacity, declaration.opacity);
            var overflowTuple = parse(context, overflow, declaration.overflow);
            this.overflowX = overflowTuple[0];
            this.overflowY = overflowTuple[overflowTuple.length > 1 ? 1 : 0];
            this.overflowWrap = parse(context, overflowWrap, declaration.overflowWrap);
            this.paddingTop = parse(context, paddingTop, declaration.paddingTop);
            this.paddingRight = parse(context, paddingRight, declaration.paddingRight);
            this.paddingBottom = parse(context, paddingBottom, declaration.paddingBottom);
            this.paddingLeft = parse(context, paddingLeft, declaration.paddingLeft);
            this.paintOrder = parse(context, paintOrder, declaration.paintOrder);
            this.position = parse(context, position, declaration.position);
            this.textAlign = parse(context, textAlign, declaration.textAlign);
            this.textDecorationColor = parse(context, textDecorationColor, (_a = declaration.textDecorationColor) !== null && _a !== void 0 ? _a : declaration.color);
            this.textDecorationLine = parse(context, textDecorationLine, (_b = declaration.textDecorationLine) !== null && _b !== void 0 ? _b : declaration.textDecoration);
            this.textShadow = parse(context, textShadow, declaration.textShadow);
            this.textTransform = parse(context, textTransform, declaration.textTransform);
            this.transform = parse(context, transform$1, declaration.transform);
            this.transformOrigin = parse(context, transformOrigin, declaration.transformOrigin);
            this.visibility = parse(context, visibility, declaration.visibility);
            this.webkitTextStrokeColor = parse(context, webkitTextStrokeColor, declaration.webkitTextStrokeColor);
            this.webkitTextStrokeWidth = parse(context, webkitTextStrokeWidth, declaration.webkitTextStrokeWidth);
            this.wordBreak = parse(context, wordBreak, declaration.wordBreak);
            this.zIndex = parse(context, zIndex, declaration.zIndex);
        }
        CSSParsedDeclaration.prototype.isVisible = function () {
            return this.display > 0 && this.opacity > 0 && this.visibility === 0 /* VISIBLE */;
        };
        CSSParsedDeclaration.prototype.isTransparent = function () {
            return isTransparent(this.backgroundColor);
        };
        CSSParsedDeclaration.prototype.isTransformed = function () {
            return this.transform !== null;
        };
        CSSParsedDeclaration.prototype.isPositioned = function () {
            return this.position !== 0 /* STATIC */;
        };
        CSSParsedDeclaration.prototype.isPositionedWithZIndex = function () {
            return this.isPositioned() && !this.zIndex.auto;
        };
        CSSParsedDeclaration.prototype.isFloating = function () {
            return this.float !== 0 /* NONE */;
        };
        CSSParsedDeclaration.prototype.isInlineLevel = function () {
            return (contains(this.display, 4 /* INLINE */) ||
                contains(this.display, 33554432 /* INLINE_BLOCK */) ||
                contains(this.display, 268435456 /* INLINE_FLEX */) ||
                contains(this.display, 536870912 /* INLINE_GRID */) ||
                contains(this.display, 67108864 /* INLINE_LIST_ITEM */) ||
                contains(this.display, 134217728 /* INLINE_TABLE */));
        };
        return CSSParsedDeclaration;
    }());
    var CSSParsedPseudoDeclaration = /** @class */ (function () {
        function CSSParsedPseudoDeclaration(context, declaration) {
            this.content = parse(context, content, declaration.content);
            this.quotes = parse(context, quotes, declaration.quotes);
        }
        return CSSParsedPseudoDeclaration;
    }());
    var CSSParsedCounterDeclaration = /** @class */ (function () {
        function CSSParsedCounterDeclaration(context, declaration) {
            this.counterIncrement = parse(context, counterIncrement, declaration.counterIncrement);
            this.counterReset = parse(context, counterReset, declaration.counterReset);
        }
        return CSSParsedCounterDeclaration;
    }());
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    var parse = function (context, descriptor, style) {
        var tokenizer = new Tokenizer();
        var value = style !== null && typeof style !== 'undefined' ? style.toString() : descriptor.initialValue;
        tokenizer.write(value);
        var parser = new Parser(tokenizer.read());
        switch (descriptor.type) {
            case 2 /* IDENT_VALUE */:
                var token = parser.parseComponentValue();
                return descriptor.parse(context, isIdentToken(token) ? token.value : descriptor.initialValue);
            case 0 /* VALUE */:
                return descriptor.parse(context, parser.parseComponentValue());
            case 1 /* LIST */:
                return descriptor.parse(context, parser.parseComponentValues());
            case 4 /* TOKEN_VALUE */:
                return parser.parseComponentValue();
            case 3 /* TYPE_VALUE */:
                switch (descriptor.format) {
                    case 'angle':
                        return angle.parse(context, parser.parseComponentValue());
                    case 'color':
                        return color$1.parse(context, parser.parseComponentValue());
                    case 'image':
                        return image.parse(context, parser.parseComponentValue());
                    case 'length':
                        var length_1 = parser.parseComponentValue();
                        return isLength(length_1) ? length_1 : ZERO_LENGTH;
                    case 'length-percentage':
                        var value_1 = parser.parseComponentValue();
                        return isLengthPercentage(value_1) ? value_1 : ZERO_LENGTH;
                    case 'time':
                        return time.parse(context, parser.parseComponentValue());
                }
                break;
        }
    };

    var elementDebuggerAttribute = 'data-html2canvas-debug';
    var getElementDebugType = function (element) {
        var attribute = element.getAttribute(elementDebuggerAttribute);
        switch (attribute) {
            case 'all':
                return 1 /* ALL */;
            case 'clone':
                return 2 /* CLONE */;
            case 'parse':
                return 3 /* PARSE */;
            case 'render':
                return 4 /* RENDER */;
            default:
                return 0 /* NONE */;
        }
    };
    var isDebugging = function (element, type) {
        var elementType = getElementDebugType(element);
        return elementType === 1 /* ALL */ || type === elementType;
    };

    var ElementContainer = /** @class */ (function () {
        function ElementContainer(context, element) {
            this.context = context;
            this.textNodes = [];
            this.elements = [];
            this.flags = 0;
            if (isDebugging(element, 3 /* PARSE */)) {
                debugger;
            }
            this.styles = new CSSParsedDeclaration(context, window.getComputedStyle(element, null));
            if (isHTMLElementNode(element)) {
                if (this.styles.animationDuration.some(function (duration) { return duration > 0; })) {
                    element.style.animationDuration = '0s';
                }
                if (this.styles.transform !== null) {
                    // getBoundingClientRect takes transforms into account
                    element.style.transform = 'none';
                }
            }
            this.bounds = parseBounds(this.context, element);
            if (isDebugging(element, 4 /* RENDER */)) {
                this.flags |= 16 /* DEBUG_RENDER */;
            }
        }
        return ElementContainer;
    }());

    /*
     * text-segmentation 1.0.3 <https://github.com/niklasvh/text-segmentation>
     * Copyright (c) 2022 Niklas von Hertzen <https://hertzen.com>
     * Released under MIT License
     */
    var base64 = 'AAAAAAAAAAAAEA4AGBkAAFAaAAACAAAAAAAIABAAGAAwADgACAAQAAgAEAAIABAACAAQAAgAEAAIABAACAAQAAgAEAAIABAAQABIAEQATAAIABAACAAQAAgAEAAIABAAVABcAAgAEAAIABAACAAQAGAAaABwAHgAgACIAI4AlgAIABAAmwCjAKgAsAC2AL4AvQDFAMoA0gBPAVYBWgEIAAgACACMANoAYgFkAWwBdAF8AX0BhQGNAZUBlgGeAaMBlQGWAasBswF8AbsBwwF0AcsBYwHTAQgA2wG/AOMBdAF8AekB8QF0AfkB+wHiAHQBfAEIAAMC5gQIAAsCEgIIAAgAFgIeAggAIgIpAggAMQI5AkACygEIAAgASAJQAlgCYAIIAAgACAAKBQoFCgUTBRMFGQUrBSsFCAAIAAgACAAIAAgACAAIAAgACABdAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACABoAmgCrwGvAQgAbgJ2AggAHgEIAAgACADnAXsCCAAIAAgAgwIIAAgACAAIAAgACACKAggAkQKZAggAPADJAAgAoQKkAqwCsgK6AsICCADJAggA0AIIAAgACAAIANYC3gIIAAgACAAIAAgACABAAOYCCAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAkASoB+QIEAAgACAA8AEMCCABCBQgACABJBVAFCAAIAAgACAAIAAgACAAIAAgACABTBVoFCAAIAFoFCABfBWUFCAAIAAgACAAIAAgAbQUIAAgACAAIAAgACABzBXsFfQWFBYoFigWKBZEFigWKBYoFmAWfBaYFrgWxBbkFCAAIAAgACAAIAAgACAAIAAgACAAIAMEFCAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAMgFCADQBQgACAAIAAgACAAIAAgACAAIAAgACAAIAO4CCAAIAAgAiQAIAAgACABAAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAD0AggACAD8AggACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIANYFCAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAMDvwAIAAgAJAIIAAgACAAIAAgACAAIAAgACwMTAwgACAB9BOsEGwMjAwgAKwMyAwsFYgE3A/MEPwMIAEUDTQNRAwgAWQOsAGEDCAAIAAgACAAIAAgACABpAzQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFOgU0BTUFNgU3BTgFOQU6BTQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFOgU0BTUFNgU3BTgFOQU6BTQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFOgU0BTUFNgU3BTgFOQU6BTQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFOgU0BTUFNgU3BTgFOQU6BTQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFOgU0BTUFNgU3BTgFOQU6BTQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFOgU0BTUFNgU3BTgFOQU6BTQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFOgU0BTUFNgU3BTgFOQU6BTQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFOgU0BTUFNgU3BTgFOQU6BTQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFOgU0BTUFNgU3BTgFOQU6BTQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFOgU0BTUFNgU3BTgFOQU6BTQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFOgU0BTUFNgU3BTgFOQU6BTQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFOgU0BTUFNgU3BTgFOQU6BTQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFOgU0BTUFNgU3BTgFOQU6BTQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFOgU0BTUFNgU3BTgFOQU6BTQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFOgU0BTUFNgU3BTgFOQU6BTQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFOgU0BTUFNgU3BTgFOQU6BTQFNQU2BTcFOAU5BToFNAU1BTYFNwU4BTkFIQUoBSwFCAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACABtAwgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACABMAEwACAAIAAgACAAIABgACAAIAAgACAC/AAgACAAyAQgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACACAAIAAwAAgACAAIAAgACAAIAAgACAAIAAAARABIAAgACAAIABQASAAIAAgAIABwAEAAjgCIABsAqAC2AL0AigDQAtwC+IJIQqVAZUBWQqVAZUBlQGVAZUBlQGrC5UBlQGVAZUBlQGVAZUBlQGVAXsKlQGVAbAK6wsrDGUMpQzlDJUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAZUBlQGVAfAKAAuZA64AtwCJALoC6ADwAAgAuACgA/oEpgO6AqsD+AAIAAgAswMIAAgACAAIAIkAuwP5AfsBwwPLAwgACAAIAAgACADRA9kDCAAIAOED6QMIAAgACAAIAAgACADuA/YDCAAIAP4DyQAIAAgABgQIAAgAXQAOBAgACAAIAAgACAAIABMECAAIAAgACAAIAAgACAD8AAQBCAAIAAgAGgQiBCoECAExBAgAEAEIAAgACAAIAAgACAAIAAgACAAIAAgACAA4BAgACABABEYECAAIAAgATAQYAQgAVAQIAAgACAAIAAgACAAIAAgACAAIAFoECAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgAOQEIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAB+BAcACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAEABhgSMBAgACAAIAAgAlAQIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAwAEAAQABAADAAMAAwADAAQABAAEAAQABAAEAAQABHATAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgAdQMIAAgACAAIAAgACAAIAMkACAAIAAgAfQMIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACACFA4kDCAAIAAgACAAIAOcBCAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAIcDCAAIAAgACAAIAAgACAAIAAgACAAIAJEDCAAIAAgACADFAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACABgBAgAZgQIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgAbAQCBXIECAAIAHkECAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACABAAJwEQACjBKoEsgQIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAC6BMIECAAIAAgACAAIAAgACABmBAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgAxwQIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAGYECAAIAAgAzgQIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgAigWKBYoFigWKBYoFigWKBd0FXwUIAOIF6gXxBYoF3gT5BQAGCAaKBYoFigWKBYoFigWKBYoFigWKBYoFigXWBIoFigWKBYoFigWKBYoFigWKBYsFEAaKBYoFigWKBYoFigWKBRQGCACKBYoFigWKBQgACAAIANEECAAIABgGigUgBggAJgYIAC4GMwaKBYoF0wQ3Bj4GigWKBYoFigWKBYoFigWKBYoFigWKBYoFigUIAAgACAAIAAgACAAIAAgAigWKBYoFigWKBYoFigWKBYoFigWKBYoFigWKBYoFigWKBYoFigWKBYoFigWKBYoFigWKBYoFigWKBYoFigWLBf///////wQABAAEAAQABAAEAAQABAAEAAQAAwAEAAQAAgAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAAAAAAAAAAAAAAAAAAAAAAAAAOAAAAAAAAAAQADgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAFAAUABQAFAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAAAAUAAAAFAAUAAAAFAAUAAAAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAEAAQABAAEAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAFAAUABQAFAAUABQAFAAUABQAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAFAAUABQAFAAUAAQAAAAUABQAFAAUABQAFAAAAAAAFAAUAAAAFAAUABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABQAFAAUABQAFAAUABQAFAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABQAFAAUABQAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAFAAAAAAAFAAUAAQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABwAFAAUABQAFAAAABwAHAAcAAAAHAAcABwAFAAEAAAAAAAAAAAAAAAAAAAAAAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHAAcABwAFAAUABQAFAAcABwAFAAUAAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHAAAAAQABAAAAAAAAAAAAAAAFAAUABQAFAAAABwAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAHAAcABwAHAAcAAAAHAAcAAAAAAAUABQAHAAUAAQAHAAEABwAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAUABQAFAAUABwABAAUABQAFAAUAAAAAAAAAAAAAAAEAAQABAAEAAQABAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABwAFAAUAAAAAAAAAAAAAAAAABQAFAAUABQAFAAUAAQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAFAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQABQANAAQABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQABAAEAAQABAAEAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAAEAAQABAAEAAQABAAEAAQABAAEAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAAQABAAEAAQABAAEAAQABAAAAAAAAAAAAAAAAAAAAAAABQAHAAUABQAFAAAAAAAAAAcABQAFAAUABQAFAAQABAAEAAQABAAEAAQABAAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAEAAQABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAFAAUAAAAFAAUABQAFAAUAAAAFAAUABQAAAAUABQAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABQAAAAAAAAAAAAUABQAFAAcAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAHAAUAAAAHAAcABwAFAAUABQAFAAUABQAFAAUABwAHAAcABwAFAAcABwAAAAUABQAFAAUABQAFAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABwAHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAAAAUABwAHAAUABQAFAAUAAAAAAAcABwAAAAAABwAHAAUAAAAAAAAAAAAAAAAAAAAAAAAABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAAAAAABQAFAAcAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAAABwAHAAcABQAFAAAAAAAAAAAABQAFAAAAAAAFAAUABQAAAAAAAAAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAAAAAAAAAFAAAAAAAAAAAAAAAAAAAAAAAAAAAABwAFAAUABQAFAAUAAAAFAAUABwAAAAcABwAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABQAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUAAAAFAAUABwAFAAUABQAFAAAAAAAHAAcAAAAAAAcABwAFAAAAAAAAAAAAAAAAAAAABQAFAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAcABwAAAAAAAAAHAAcABwAAAAcABwAHAAUAAAAAAAAAAAAAAAAAAAAAAAAABQAAAAAAAAAAAAAAAAAAAAAABQAHAAcABwAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABwAHAAcABwAAAAUABQAFAAAABQAFAAUABQAAAAAAAAAAAAAAAAAAAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAAAAcABQAHAAcABQAHAAcAAAAFAAcABwAAAAcABwAFAAUAAAAAAAAAAAAAAAAAAAAFAAUAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAcABwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAAAAUABwAAAAAAAAAAAAAAAAAAAAAAAAAAAAUAAAAAAAAAAAAFAAcABwAFAAUABQAAAAUAAAAHAAcABwAHAAcABwAHAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUAAAAHAAUABQAFAAUABQAFAAUAAAAAAAAAAAAAAAAAAAAAAAUABQAFAAUABQAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAAABwAFAAUABQAFAAUABQAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAUABQAFAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAAAAUAAAAFAAAAAAAAAAAABwAHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABwAFAAUABQAFAAUAAAAFAAUAAAAAAAAAAAAAAAUABQAFAAUABQAFAAUABQAFAAUABQAAAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAFAAUABwAFAAUABQAFAAUABQAAAAUABQAHAAcABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHAAcABQAFAAAAAAAAAAAABQAFAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAAAAcABQAFAAAAAAAAAAAAAAAAAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAHAAUABQAFAAUABQAFAAUABwAHAAcABwAHAAcABwAHAAUABwAHAAUABQAFAAUABQAFAAUABQAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAUABwAHAAcABwAFAAUABwAHAAcAAAAAAAAAAAAHAAcABQAHAAcABwAHAAcABwAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAcABwAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAcABQAHAAUABQAFAAUABQAFAAUAAAAFAAAABQAAAAAABQAFAAUABQAFAAUABQAFAAcABwAHAAcABwAHAAUABQAFAAUABQAFAAUABQAFAAUAAAAAAAUABQAFAAUABQAHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAFAAUABQAFAAUABwAFAAcABwAHAAcABwAFAAcABwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABQAFAAUABQAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHAAUABQAFAAUABwAHAAUABQAHAAUABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAcABQAFAAcABwAHAAUABwAFAAUABQAHAAcAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABwAHAAcABwAHAAcABwAHAAUABQAFAAUABQAFAAUABQAHAAcABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAUAAAAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAcABQAFAAUABQAFAAUABQAAAAAAAAAAAAUAAAAAAAAAAAAAAAAABQAAAAAABwAFAAUAAAAAAAAAAAAAAAAABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAAABQAFAAUABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAUABQAFAAUADgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAFAAUAAAAFAAUABQAFAAUABQAFAAUABQAFAAAAAAAAAAAABQAAAAAAAAAFAAAAAAAAAAAABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABwAHAAUABQAHAAAAAAAAAAAABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAcABwAHAAcABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUAAAAAAAAAAAAAAAAABQAFAAUABQAFAAUABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAFAAUABQAFAAUABQAFAAUABQAHAAcAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAcABwAFAAUABQAFAAcABwAFAAUABwAHAAAAAAAAAAAAAAAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAFAAUABQAFAAcABwAFAAUABwAHAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAAAAAAAAAAAAAAAAAAAAAAFAAcAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUAAAAFAAUABQAAAAAABQAFAAAAAAAAAAAAAAAFAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAcABQAFAAcABwAAAAAAAAAAAAAABwAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAcABwAFAAcABwAFAAcABwAAAAcABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAFAAUABQAAAAAAAAAAAAAAAAAFAAUABQAAAAUABQAAAAAAAAAAAAAABQAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABQAAAAAAAAAAAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAcABQAHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABQAFAAUABwAFAAUABQAFAAUABQAFAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABwAHAAcABQAFAAUABQAFAAUABQAFAAUABwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHAAcABwAFAAUABQAHAAcABQAHAAUABQAAAAAAAAAAAAAAAAAFAAAABwAHAAcABQAFAAUABQAFAAUABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABwAHAAcABwAAAAAABwAHAAAAAAAHAAcABwAAAAAAAAAAAAAAAAAAAAAAAAAFAAAAAAAAAAAAAAAAAAAAAAAAAAAABwAHAAAAAAAFAAUABQAFAAUABQAFAAAAAAAAAAUABQAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHAAcABwAFAAUABQAFAAUABQAFAAUABwAHAAUABQAFAAcABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAHAAcABQAFAAUABQAFAAUABwAFAAcABwAFAAcABQAFAAcABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAHAAcABQAFAAUABQAAAAAABwAHAAcABwAFAAUABwAFAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAcABwAHAAUABQAFAAUABQAFAAUABQAHAAcABQAHAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABwAFAAcABwAFAAUABQAFAAUABQAHAAUAAAAAAAAAAAAAAAAAAAAAAAcABwAFAAUABQAFAAcABQAFAAUABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHAAcABwAFAAUABQAFAAUABQAFAAUABQAHAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHAAcABwAFAAUABQAFAAAAAAAFAAUABwAHAAcABwAFAAAAAAAAAAcAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABQAFAAUABQAFAAUABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAUABQAFAAUABwAHAAUABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAcABQAFAAUABQAFAAUABQAAAAUABQAFAAUABQAFAAcABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUAAAAHAAUABQAFAAUABQAFAAUABwAFAAUABwAFAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAUABQAFAAUAAAAAAAAABQAAAAUABQAAAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAHAAcABwAHAAcAAAAFAAUAAAAHAAcABQAHAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABwAHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABQAFAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABQAFAAUABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAAAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAAAAAAAAAAAAAAAAAAABQAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAcABwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAAAAUABQAFAAAAAAAFAAUABQAFAAUABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAAAAAAAAAAABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAAAAAAAAAAAAAAAAAAAAAAFAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUABQAFAAUABQAAAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAUABQAFAAUABQAAAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAFAAUABQAAAAAABQAFAAUABQAFAAUABQAAAAUABQAAAAUABQAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAUABQAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAFAAUABQAFAAUABQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAFAAUABQAFAAUADgAOAA4ADgAOAA4ADwAPAA8ADwAPAA8ADwAPAA8ADwAPAA8ADwAPAA8ADwAPAA8ADwAPAA8ADwAPAA8ADwAPAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAcABwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABwAHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAcABwAHAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAgACAAIAAAAAAAAAAkACQAJAAkACQAJAAkACQAJAAkACQAJAAkACQAJAAkACQAJAAkACQAJAAkACQAJAAkACQAJAAkACQAJAAkACQAKAAoACgAKAAoACgAKAAoACgAKAAoACgAKAAoACgAKAAoACgAKAAoACgAKAAoACgAMAAwADAAMAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAkACQAJAAkACQAJAAkACQAJAAkACQAJAAkACQAJAAkACQAJAAkAAAAAAAAAAAAKAAoACgAKAAoACgAKAAoACgAKAAoACgAKAAoACgAKAAoACgAKAAoACgAKAAoACgAKAAoACgAKAAoACgAKAAoACgAAAAAAAAAAAAsADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwACwAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAMAAwADAAAAAAADgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOAA4ADgAOAA4ADgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAOAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA4ADgAAAAAAAAAAAAAAAAAAAAAADgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAOAA4ADgAOAA4ADgAOAA4ADgAOAAAAAAAAAAAADgAOAA4AAAAAAAAAAAAAAAAAAAAOAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAOAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAAAAAAAAAAAAAAAAAAAAAAAAAOAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAOAA4ADgAAAA4ADgAOAA4ADgAOAAAADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4AAAAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4AAAAAAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAAAA4AAAAOAAAAAAAAAAAAAAAAAA4AAAAAAAAAAAAAAAAADgAAAAAAAAAAAAAAAAAAAAAAAAAAAA4ADgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAAAAAADgAAAAAAAAAAAA4AAAAOAAAAAAAAAAAADgAOAA4AAAAOAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOAA4ADgAOAA4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOAA4ADgAAAAAAAAAAAAAAAAAAAAAAAAAOAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOAA4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA4ADgAOAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAOAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAAAAAAAAAAAA4AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOAAAADgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOAA4ADgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA4ADgAOAA4ADgAOAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA4ADgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAAAAAADgAOAA4ADgAOAA4ADgAOAA4ADgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAAAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA4AAAAAAA4ADgAOAA4ADgAOAA4ADgAOAAAADgAOAA4ADgAAAAAAAAAAAAAAAAAAAAAAAAAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4AAAAAAAAAAAAAAAAADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOAA4ADgAOAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAOAA4ADgAOAA4ADgAOAAAAAAAAAAAAAAAAAAAAAAAAAAAADgAOAA4ADgAOAA4AAAAAAAAAAAAAAAAAAAAAAA4ADgAOAA4ADgAOAA4ADgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4AAAAOAA4ADgAOAA4ADgAAAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4ADgAOAA4AAAAAAAAAAAA=';

    /*
     * utrie 1.0.2 <https://github.com/niklasvh/utrie>
     * Copyright (c) 2022 Niklas von Hertzen <https://hertzen.com>
     * Released under MIT License
     */
    var chars$1 = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
    // Use a lookup table to find the index.
    var lookup$1 = typeof Uint8Array === 'undefined' ? [] : new Uint8Array(256);
    for (var i$1 = 0; i$1 < chars$1.length; i$1++) {
        lookup$1[chars$1.charCodeAt(i$1)] = i$1;
    }
    var decode = function (base64) {
        var bufferLength = base64.length * 0.75, len = base64.length, i, p = 0, encoded1, encoded2, encoded3, encoded4;
        if (base64[base64.length - 1] === '=') {
            bufferLength--;
            if (base64[base64.length - 2] === '=') {
                bufferLength--;
            }
        }
        var buffer = typeof ArrayBuffer !== 'undefined' &&
            typeof Uint8Array !== 'undefined' &&
            typeof Uint8Array.prototype.slice !== 'undefined'
            ? new ArrayBuffer(bufferLength)
            : new Array(bufferLength);
        var bytes = Array.isArray(buffer) ? buffer : new Uint8Array(buffer);
        for (i = 0; i < len; i += 4) {
            encoded1 = lookup$1[base64.charCodeAt(i)];
            encoded2 = lookup$1[base64.charCodeAt(i + 1)];
            encoded3 = lookup$1[base64.charCodeAt(i + 2)];
            encoded4 = lookup$1[base64.charCodeAt(i + 3)];
            bytes[p++] = (encoded1 << 2) | (encoded2 >> 4);
            bytes[p++] = ((encoded2 & 15) << 4) | (encoded3 >> 2);
            bytes[p++] = ((encoded3 & 3) << 6) | (encoded4 & 63);
        }
        return buffer;
    };
    var polyUint16Array = function (buffer) {
        var length = buffer.length;
        var bytes = [];
        for (var i = 0; i < length; i += 2) {
            bytes.push((buffer[i + 1] << 8) | buffer[i]);
        }
        return bytes;
    };
    var polyUint32Array = function (buffer) {
        var length = buffer.length;
        var bytes = [];
        for (var i = 0; i < length; i += 4) {
            bytes.push((buffer[i + 3] << 24) | (buffer[i + 2] << 16) | (buffer[i + 1] << 8) | buffer[i]);
        }
        return bytes;
    };

    /** Shift size for getting the index-2 table offset. */
    var UTRIE2_SHIFT_2 = 5;
    /** Shift size for getting the index-1 table offset. */
    var UTRIE2_SHIFT_1 = 6 + 5;
    /**
     * Shift size for shifting left the index array values.
     * Increases possible data size with 16-bit index values at the cost
     * of compactability.
     * This requires data blocks to be aligned by UTRIE2_DATA_GRANULARITY.
     */
    var UTRIE2_INDEX_SHIFT = 2;
    /**
     * Difference between the two shift sizes,
     * for getting an index-1 offset from an index-2 offset. 6=11-5
     */
    var UTRIE2_SHIFT_1_2 = UTRIE2_SHIFT_1 - UTRIE2_SHIFT_2;
    /**
     * The part of the index-2 table for U+D800..U+DBFF stores values for
     * lead surrogate code _units_ not code _points_.
     * Values for lead surrogate code _points_ are indexed with this portion of the table.
     * Length=32=0x20=0x400>>UTRIE2_SHIFT_2. (There are 1024=0x400 lead surrogates.)
     */
    var UTRIE2_LSCP_INDEX_2_OFFSET = 0x10000 >> UTRIE2_SHIFT_2;
    /** Number of entries in a data block. 32=0x20 */
    var UTRIE2_DATA_BLOCK_LENGTH = 1 << UTRIE2_SHIFT_2;
    /** Mask for getting the lower bits for the in-data-block offset. */
    var UTRIE2_DATA_MASK = UTRIE2_DATA_BLOCK_LENGTH - 1;
    var UTRIE2_LSCP_INDEX_2_LENGTH = 0x400 >> UTRIE2_SHIFT_2;
    /** Count the lengths of both BMP pieces. 2080=0x820 */
    var UTRIE2_INDEX_2_BMP_LENGTH = UTRIE2_LSCP_INDEX_2_OFFSET + UTRIE2_LSCP_INDEX_2_LENGTH;
    /**
     * The 2-byte UTF-8 version of the index-2 table follows at offset 2080=0x820.
     * Length 32=0x20 for lead bytes C0..DF, regardless of UTRIE2_SHIFT_2.
     */
    var UTRIE2_UTF8_2B_INDEX_2_OFFSET = UTRIE2_INDEX_2_BMP_LENGTH;
    var UTRIE2_UTF8_2B_INDEX_2_LENGTH = 0x800 >> 6; /* U+0800 is the first code point after 2-byte UTF-8 */
    /**
     * The index-1 table, only used for supplementary code points, at offset 2112=0x840.
     * Variable length, for code points up to highStart, where the last single-value range starts.
     * Maximum length 512=0x200=0x100000>>UTRIE2_SHIFT_1.
     * (For 0x100000 supplementary code points U+10000..U+10ffff.)
     *
     * The part of the index-2 table for supplementary code points starts
     * after this index-1 table.
     *
     * Both the index-1 table and the following part of the index-2 table
     * are omitted completely if there is only BMP data.
     */
    var UTRIE2_INDEX_1_OFFSET = UTRIE2_UTF8_2B_INDEX_2_OFFSET + UTRIE2_UTF8_2B_INDEX_2_LENGTH;
    /**
     * Number of index-1 entries for the BMP. 32=0x20
     * This part of the index-1 table is omitted from the serialized form.
     */
    var UTRIE2_OMITTED_BMP_INDEX_1_LENGTH = 0x10000 >> UTRIE2_SHIFT_1;
    /** Number of entries in an index-2 block. 64=0x40 */
    var UTRIE2_INDEX_2_BLOCK_LENGTH = 1 << UTRIE2_SHIFT_1_2;
    /** Mask for getting the lower bits for the in-index-2-block offset. */
    var UTRIE2_INDEX_2_MASK = UTRIE2_INDEX_2_BLOCK_LENGTH - 1;
    var slice16 = function (view, start, end) {
        if (view.slice) {
            return view.slice(start, end);
        }
        return new Uint16Array(Array.prototype.slice.call(view, start, end));
    };
    var slice32 = function (view, start, end) {
        if (view.slice) {
            return view.slice(start, end);
        }
        return new Uint32Array(Array.prototype.slice.call(view, start, end));
    };
    var createTrieFromBase64 = function (base64, _byteLength) {
        var buffer = decode(base64);
        var view32 = Array.isArray(buffer) ? polyUint32Array(buffer) : new Uint32Array(buffer);
        var view16 = Array.isArray(buffer) ? polyUint16Array(buffer) : new Uint16Array(buffer);
        var headerLength = 24;
        var index = slice16(view16, headerLength / 2, view32[4] / 2);
        var data = view32[5] === 2
            ? slice16(view16, (headerLength + view32[4]) / 2)
            : slice32(view32, Math.ceil((headerLength + view32[4]) / 4));
        return new Trie(view32[0], view32[1], view32[2], view32[3], index, data);
    };
    var Trie = /** @class */ (function () {
        function Trie(initialValue, errorValue, highStart, highValueIndex, index, data) {
            this.initialValue = initialValue;
            this.errorValue = errorValue;
            this.highStart = highStart;
            this.highValueIndex = highValueIndex;
            this.index = index;
            this.data = data;
        }
        /**
         * Get the value for a code point as stored in the Trie.
         *
         * @param codePoint the code point
         * @return the value
         */
        Trie.prototype.get = function (codePoint) {
            var ix;
            if (codePoint >= 0) {
                if (codePoint < 0x0d800 || (codePoint > 0x0dbff && codePoint <= 0x0ffff)) {
                    // Ordinary BMP code point, excluding leading surrogates.
                    // BMP uses a single level lookup.  BMP index starts at offset 0 in the Trie2 index.
                    // 16 bit data is stored in the index array itself.
                    ix = this.index[codePoint >> UTRIE2_SHIFT_2];
                    ix = (ix << UTRIE2_INDEX_SHIFT) + (codePoint & UTRIE2_DATA_MASK);
                    return this.data[ix];
                }
                if (codePoint <= 0xffff) {
                    // Lead Surrogate Code Point.  A Separate index section is stored for
                    // lead surrogate code units and code points.
                    //   The main index has the code unit data.
                    //   For this function, we need the code point data.
                    // Note: this expression could be refactored for slightly improved efficiency, but
                    //       surrogate code points will be so rare in practice that it's not worth it.
                    ix = this.index[UTRIE2_LSCP_INDEX_2_OFFSET + ((codePoint - 0xd800) >> UTRIE2_SHIFT_2)];
                    ix = (ix << UTRIE2_INDEX_SHIFT) + (codePoint & UTRIE2_DATA_MASK);
                    return this.data[ix];
                }
                if (codePoint < this.highStart) {
                    // Supplemental code point, use two-level lookup.
                    ix = UTRIE2_INDEX_1_OFFSET - UTRIE2_OMITTED_BMP_INDEX_1_LENGTH + (codePoint >> UTRIE2_SHIFT_1);
                    ix = this.index[ix];
                    ix += (codePoint >> UTRIE2_SHIFT_2) & UTRIE2_INDEX_2_MASK;
                    ix = this.index[ix];
                    ix = (ix << UTRIE2_INDEX_SHIFT) + (codePoint & UTRIE2_DATA_MASK);
                    return this.data[ix];
                }
                if (codePoint <= 0x10ffff) {
                    return this.data[this.highValueIndex];
                }
            }
            // Fall through.  The code point is outside of the legal range of 0..0x10ffff.
            return this.errorValue;
        };
        return Trie;
    }());

    /*
     * base64-arraybuffer 1.0.2 <https://github.com/niklasvh/base64-arraybuffer>
     * Copyright (c) 2022 Niklas von Hertzen <https://hertzen.com>
     * Released under MIT License
     */
    var chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
    // Use a lookup table to find the index.
    var lookup = typeof Uint8Array === 'undefined' ? [] : new Uint8Array(256);
    for (var i = 0; i < chars.length; i++) {
        lookup[chars.charCodeAt(i)] = i;
    }

    var Prepend = 1;
    var CR = 2;
    var LF = 3;
    var Control = 4;
    var Extend = 5;
    var SpacingMark = 7;
    var L = 8;
    var V = 9;
    var T = 10;
    var LV = 11;
    var LVT = 12;
    var ZWJ = 13;
    var Extended_Pictographic = 14;
    var RI = 15;
    var toCodePoints = function (str) {
        var codePoints = [];
        var i = 0;
        var length = str.length;
        while (i < length) {
            var value = str.charCodeAt(i++);
            if (value >= 0xd800 && value <= 0xdbff && i < length) {
                var extra = str.charCodeAt(i++);
                if ((extra & 0xfc00) === 0xdc00) {
                    codePoints.push(((value & 0x3ff) << 10) + (extra & 0x3ff) + 0x10000);
                }
                else {
                    codePoints.push(value);
                    i--;
                }
            }
            else {
                codePoints.push(value);
            }
        }
        return codePoints;
    };
    var fromCodePoint = function () {
        var codePoints = [];
        for (var _i = 0; _i < arguments.length; _i++) {
            codePoints[_i] = arguments[_i];
        }
        if (String.fromCodePoint) {
            return String.fromCodePoint.apply(String, codePoints);
        }
        var length = codePoints.length;
        if (!length) {
            return '';
        }
        var codeUnits = [];
        var index = -1;
        var result = '';
        while (++index < length) {
            var codePoint = codePoints[index];
            if (codePoint <= 0xffff) {
                codeUnits.push(codePoint);
            }
            else {
                codePoint -= 0x10000;
                codeUnits.push((codePoint >> 10) + 0xd800, (codePoint % 0x400) + 0xdc00);
            }
            if (index + 1 === length || codeUnits.length > 0x4000) {
                result += String.fromCharCode.apply(String, codeUnits);
                codeUnits.length = 0;
            }
        }
        return result;
    };
    var UnicodeTrie = createTrieFromBase64(base64);
    var BREAK_NOT_ALLOWED = '×';
    var BREAK_ALLOWED = '÷';
    var codePointToClass = function (codePoint) { return UnicodeTrie.get(codePoint); };
    var _graphemeBreakAtIndex = function (_codePoints, classTypes, index) {
        var prevIndex = index - 2;
        var prev = classTypes[prevIndex];
        var current = classTypes[index - 1];
        var next = classTypes[index];
        // GB3 Do not break between a CR and LF
        if (current === CR && next === LF) {
            return BREAK_NOT_ALLOWED;
        }
        // GB4 Otherwise, break before and after controls.
        if (current === CR || current === LF || current === Control) {
            return BREAK_ALLOWED;
        }
        // GB5
        if (next === CR || next === LF || next === Control) {
            return BREAK_ALLOWED;
        }
        // Do not break Hangul syllable sequences.
        // GB6
        if (current === L && [L, V, LV, LVT].indexOf(next) !== -1) {
            return BREAK_NOT_ALLOWED;
        }
        // GB7
        if ((current === LV || current === V) && (next === V || next === T)) {
            return BREAK_NOT_ALLOWED;
        }
        // GB8
        if ((current === LVT || current === T) && next === T) {
            return BREAK_NOT_ALLOWED;
        }
        // GB9 Do not break before extending characters or ZWJ.
        if (next === ZWJ || next === Extend) {
            return BREAK_NOT_ALLOWED;
        }
        // Do not break before SpacingMarks, or after Prepend characters.
        // GB9a
        if (next === SpacingMark) {
            return BREAK_NOT_ALLOWED;
        }
        // GB9a
        if (current === Prepend) {
            return BREAK_NOT_ALLOWED;
        }
        // GB11 Do not break within emoji modifier sequences or emoji zwj sequences.
        if (current === ZWJ && next === Extended_Pictographic) {
            while (prev === Extend) {
                prev = classTypes[--prevIndex];
            }
            if (prev === Extended_Pictographic) {
                return BREAK_NOT_ALLOWED;
            }
        }
        // GB12 Do not break within emoji flag sequences.
        // That is, do not break between regional indicator (RI) symbols
        // if there is an odd number of RI characters before the break point.
        if (current === RI && next === RI) {
            var countRI = 0;
            while (prev === RI) {
                countRI++;
                prev = classTypes[--prevIndex];
            }
            if (countRI % 2 === 0) {
                return BREAK_NOT_ALLOWED;
            }
        }
        return BREAK_ALLOWED;
    };
    var GraphemeBreaker = function (str) {
        var codePoints = toCodePoints(str);
        var length = codePoints.length;
        var index = 0;
        var lastEnd = 0;
        var classTypes = codePoints.map(codePointToClass);
        return {
            next: function () {
                if (index >= length) {
                    return { done: true, value: null };
                }
                var graphemeBreak = BREAK_NOT_ALLOWED;
                while (index < length &&
                    (graphemeBreak = _graphemeBreakAtIndex(codePoints, classTypes, ++index)) === BREAK_NOT_ALLOWED) { }
                if (graphemeBreak !== BREAK_NOT_ALLOWED || index === length) {
                    var value = fromCodePoint.apply(null, codePoints.slice(lastEnd, index));
                    lastEnd = index;
                    return { value: value, done: false };
                }
                return { done: true, value: null };
            },
        };
    };
    var splitGraphemes = function (str) {
        var breaker = GraphemeBreaker(str);
        var graphemes = [];
        var bk;
        while (!(bk = breaker.next()).done) {
            if (bk.value) {
                graphemes.push(bk.value.slice());
            }
        }
        return graphemes;
    };

    var testRangeBounds = function (document) {
        var TEST_HEIGHT = 123;
        if (document.createRange) {
            var range = document.createRange();
            if (range.getBoundingClientRect) {
                var testElement = document.createElement('boundtest');
                testElement.style.height = TEST_HEIGHT + "px";
                testElement.style.display = 'block';
                document.body.appendChild(testElement);
                range.selectNode(testElement);
                var rangeBounds = range.getBoundingClientRect();
                var rangeHeight = Math.round(rangeBounds.height);
                document.body.removeChild(testElement);
                if (rangeHeight === TEST_HEIGHT) {
                    return true;
                }
            }
        }
        return false;
    };
    var testIOSLineBreak = function (document) {
        var testElement = document.createElement('boundtest');
        testElement.style.width = '50px';
        testElement.style.display = 'block';
        testElement.style.fontSize = '12px';
        testElement.style.letterSpacing = '0px';
        testElement.style.wordSpacing = '0px';
        document.body.appendChild(testElement);
        var range = document.createRange();
        testElement.innerHTML = typeof ''.repeat === 'function' ? '&#128104;'.repeat(10) : '';
        var node = testElement.firstChild;
        var textList = toCodePoints$1(node.data).map(function (i) { return fromCodePoint$1(i); });
        var offset = 0;
        var prev = {};
        // ios 13 does not handle range getBoundingClientRect line changes correctly #2177
        var supports = textList.every(function (text, i) {
            range.setStart(node, offset);
            range.setEnd(node, offset + text.length);
            var rect = range.getBoundingClientRect();
            offset += text.length;
            var boundAhead = rect.x > prev.x || rect.y > prev.y;
            prev = rect;
            if (i === 0) {
                return true;
            }
            return boundAhead;
        });
        document.body.removeChild(testElement);
        return supports;
    };
    var testCORS = function () { return typeof new Image().crossOrigin !== 'undefined'; };
    var testResponseType = function () { return typeof new XMLHttpRequest().responseType === 'string'; };
    var testSVG = function (document) {
        var img = new Image();
        var canvas = document.createElement('canvas');
        var ctx = canvas.getContext('2d');
        if (!ctx) {
            return false;
        }
        img.src = "data:image/svg+xml,<svg xmlns='http://www.w3.org/2000/svg'></svg>";
        try {
            ctx.drawImage(img, 0, 0);
            canvas.toDataURL();
        }
        catch (e) {
            return false;
        }
        return true;
    };
    var isGreenPixel = function (data) {
        return data[0] === 0 && data[1] === 255 && data[2] === 0 && data[3] === 255;
    };
    var testForeignObject = function (document) {
        var canvas = document.createElement('canvas');
        var size = 100;
        canvas.width = size;
        canvas.height = size;
        var ctx = canvas.getContext('2d');
        if (!ctx) {
            return Promise.reject(false);
        }
        ctx.fillStyle = 'rgb(0, 255, 0)';
        ctx.fillRect(0, 0, size, size);
        var img = new Image();
        var greenImageSrc = canvas.toDataURL();
        img.src = greenImageSrc;
        var svg = createForeignObjectSVG(size, size, 0, 0, img);
        ctx.fillStyle = 'red';
        ctx.fillRect(0, 0, size, size);
        return loadSerializedSVG$1(svg)
            .then(function (img) {
            ctx.drawImage(img, 0, 0);
            var data = ctx.getImageData(0, 0, size, size).data;
            ctx.fillStyle = 'red';
            ctx.fillRect(0, 0, size, size);
            var node = document.createElement('div');
            node.style.backgroundImage = "url(" + greenImageSrc + ")";
            node.style.height = size + "px";
            // Firefox 55 does not render inline <img /> tags
            return isGreenPixel(data)
                ? loadSerializedSVG$1(createForeignObjectSVG(size, size, 0, 0, node))
                : Promise.reject(false);
        })
            .then(function (img) {
            ctx.drawImage(img, 0, 0);
            // Edge does not render background-images
            return isGreenPixel(ctx.getImageData(0, 0, size, size).data);
        })
            .catch(function () { return false; });
    };
    var createForeignObjectSVG = function (width, height, x, y, node) {
        var xmlns = 'http://www.w3.org/2000/svg';
        var svg = document.createElementNS(xmlns, 'svg');
        var foreignObject = document.createElementNS(xmlns, 'foreignObject');
        svg.setAttributeNS(null, 'width', width.toString());
        svg.setAttributeNS(null, 'height', height.toString());
        foreignObject.setAttributeNS(null, 'width', '100%');
        foreignObject.setAttributeNS(null, 'height', '100%');
        foreignObject.setAttributeNS(null, 'x', x.toString());
        foreignObject.setAttributeNS(null, 'y', y.toString());
        foreignObject.setAttributeNS(null, 'externalResourcesRequired', 'true');
        svg.appendChild(foreignObject);
        foreignObject.appendChild(node);
        return svg;
    };
    var loadSerializedSVG$1 = function (svg) {
        return new Promise(function (resolve, reject) {
            var img = new Image();
            img.onload = function () { return resolve(img); };
            img.onerror = reject;
            img.src = "data:image/svg+xml;charset=utf-8," + encodeURIComponent(new XMLSerializer().serializeToString(svg));
        });
    };
    var FEATURES = {
        get SUPPORT_RANGE_BOUNDS() {
            var value = testRangeBounds(document);
            Object.defineProperty(FEATURES, 'SUPPORT_RANGE_BOUNDS', { value: value });
            return value;
        },
        get SUPPORT_WORD_BREAKING() {
            var value = FEATURES.SUPPORT_RANGE_BOUNDS && testIOSLineBreak(document);
            Object.defineProperty(FEATURES, 'SUPPORT_WORD_BREAKING', { value: value });
            return value;
        },
        get SUPPORT_SVG_DRAWING() {
            var value = testSVG(document);
            Object.defineProperty(FEATURES, 'SUPPORT_SVG_DRAWING', { value: value });
            return value;
        },
        get SUPPORT_FOREIGNOBJECT_DRAWING() {
            var value = typeof Array.from === 'function' && typeof window.fetch === 'function'
                ? testForeignObject(document)
                : Promise.resolve(false);
            Object.defineProperty(FEATURES, 'SUPPORT_FOREIGNOBJECT_DRAWING', { value: value });
            return value;
        },
        get SUPPORT_CORS_IMAGES() {
            var value = testCORS();
            Object.defineProperty(FEATURES, 'SUPPORT_CORS_IMAGES', { value: value });
            return value;
        },
        get SUPPORT_RESPONSE_TYPE() {
            var value = testResponseType();
            Object.defineProperty(FEATURES, 'SUPPORT_RESPONSE_TYPE', { value: value });
            return value;
        },
        get SUPPORT_CORS_XHR() {
            var value = 'withCredentials' in new XMLHttpRequest();
            Object.defineProperty(FEATURES, 'SUPPORT_CORS_XHR', { value: value });
            return value;
        },
        get SUPPORT_NATIVE_TEXT_SEGMENTATION() {
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            var value = !!(typeof Intl !== 'undefined' && Intl.Segmenter);
            Object.defineProperty(FEATURES, 'SUPPORT_NATIVE_TEXT_SEGMENTATION', { value: value });
            return value;
        }
    };

    var TextBounds = /** @class */ (function () {
        function TextBounds(text, bounds) {
            this.text = text;
            this.bounds = bounds;
        }
        return TextBounds;
    }());
    var parseTextBounds = function (context, value, styles, node) {
        var textList = breakText(value, styles);
        var textBounds = [];
        var offset = 0;
        textList.forEach(function (text) {
            if (styles.textDecorationLine.length || text.trim().length > 0) {
                if (FEATURES.SUPPORT_RANGE_BOUNDS) {
                    var clientRects = createRange(node, offset, text.length).getClientRects();
                    if (clientRects.length > 1) {
                        var subSegments = segmentGraphemes(text);
                        var subOffset_1 = 0;
                        subSegments.forEach(function (subSegment) {
                            textBounds.push(new TextBounds(subSegment, Bounds.fromDOMRectList(context, createRange(node, subOffset_1 + offset, subSegment.length).getClientRects())));
                            subOffset_1 += subSegment.length;
                        });
                    }
                    else {
                        textBounds.push(new TextBounds(text, Bounds.fromDOMRectList(context, clientRects)));
                    }
                }
                else {
                    var replacementNode = node.splitText(text.length);
                    textBounds.push(new TextBounds(text, getWrapperBounds(context, node)));
                    node = replacementNode;
                }
            }
            else if (!FEATURES.SUPPORT_RANGE_BOUNDS) {
                node = node.splitText(text.length);
            }
            offset += text.length;
        });
        return textBounds;
    };
    var getWrapperBounds = function (context, node) {
        var ownerDocument = node.ownerDocument;
        if (ownerDocument) {
            var wrapper = ownerDocument.createElement('html2canvaswrapper');
            wrapper.appendChild(node.cloneNode(true));
            var parentNode = node.parentNode;
            if (parentNode) {
                parentNode.replaceChild(wrapper, node);
                var bounds = parseBounds(context, wrapper);
                if (wrapper.firstChild) {
                    parentNode.replaceChild(wrapper.firstChild, wrapper);
                }
                return bounds;
            }
        }
        return Bounds.EMPTY;
    };
    var createRange = function (node, offset, length) {
        var ownerDocument = node.ownerDocument;
        if (!ownerDocument) {
            throw new Error('Node has no owner document');
        }
        var range = ownerDocument.createRange();
        range.setStart(node, offset);
        range.setEnd(node, offset + length);
        return range;
    };
    var segmentGraphemes = function (value) {
        if (FEATURES.SUPPORT_NATIVE_TEXT_SEGMENTATION) {
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            var segmenter = new Intl.Segmenter(void 0, { granularity: 'grapheme' });
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            return Array.from(segmenter.segment(value)).map(function (segment) { return segment.segment; });
        }
        return splitGraphemes(value);
    };
    var segmentWords = function (value, styles) {
        if (FEATURES.SUPPORT_NATIVE_TEXT_SEGMENTATION) {
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            var segmenter = new Intl.Segmenter(void 0, {
                granularity: 'word'
            });
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            return Array.from(segmenter.segment(value)).map(function (segment) { return segment.segment; });
        }
        return breakWords(value, styles);
    };
    var breakText = function (value, styles) {
        return styles.letterSpacing !== 0 ? segmentGraphemes(value) : segmentWords(value, styles);
    };
    // https://drafts.csswg.org/css-text/#word-separator
    var wordSeparators = [0x0020, 0x00a0, 0x1361, 0x10100, 0x10101, 0x1039, 0x1091];
    var breakWords = function (str, styles) {
        var breaker = LineBreaker(str, {
            lineBreak: styles.lineBreak,
            wordBreak: styles.overflowWrap === "break-word" /* BREAK_WORD */ ? 'break-word' : styles.wordBreak
        });
        var words = [];
        var bk;
        var _loop_1 = function () {
            if (bk.value) {
                var value = bk.value.slice();
                var codePoints = toCodePoints$1(value);
                var word_1 = '';
                codePoints.forEach(function (codePoint) {
                    if (wordSeparators.indexOf(codePoint) === -1) {
                        word_1 += fromCodePoint$1(codePoint);
                    }
                    else {
                        if (word_1.length) {
                            words.push(word_1);
                        }
                        words.push(fromCodePoint$1(codePoint));
                        word_1 = '';
                    }
                });
                if (word_1.length) {
                    words.push(word_1);
                }
            }
        };
        while (!(bk = breaker.next()).done) {
            _loop_1();
        }
        return words;
    };

    var TextContainer = /** @class */ (function () {
        function TextContainer(context, node, styles) {
            this.text = transform(node.data, styles.textTransform);
            this.textBounds = parseTextBounds(context, this.text, styles, node);
        }
        return TextContainer;
    }());
    var transform = function (text, transform) {
        switch (transform) {
            case 1 /* LOWERCASE */:
                return text.toLowerCase();
            case 3 /* CAPITALIZE */:
                return text.replace(CAPITALIZE, capitalize);
            case 2 /* UPPERCASE */:
                return text.toUpperCase();
            default:
                return text;
        }
    };
    var CAPITALIZE = /(^|\s|:|-|\(|\))([a-z])/g;
    var capitalize = function (m, p1, p2) {
        if (m.length > 0) {
            return p1 + p2.toUpperCase();
        }
        return m;
    };

    var ImageElementContainer = /** @class */ (function (_super) {
        __extends(ImageElementContainer, _super);
        function ImageElementContainer(context, img) {
            var _this = _super.call(this, context, img) || this;
            _this.src = img.currentSrc || img.src;
            _this.intrinsicWidth = img.naturalWidth;
            _this.intrinsicHeight = img.naturalHeight;
            _this.context.cache.addImage(_this.src);
            return _this;
        }
        return ImageElementContainer;
    }(ElementContainer));

    var CanvasElementContainer = /** @class */ (function (_super) {
        __extends(CanvasElementContainer, _super);
        function CanvasElementContainer(context, canvas) {
            var _this = _super.call(this, context, canvas) || this;
            _this.canvas = canvas;
            _this.intrinsicWidth = canvas.width;
            _this.intrinsicHeight = canvas.height;
            return _this;
        }
        return CanvasElementContainer;
    }(ElementContainer));

    var SVGElementContainer = /** @class */ (function (_super) {
        __extends(SVGElementContainer, _super);
        function SVGElementContainer(context, img) {
            var _this = _super.call(this, context, img) || this;
            var s = new XMLSerializer();
            var bounds = parseBounds(context, img);
            img.setAttribute('width', bounds.width + "px");
            img.setAttribute('height', bounds.height + "px");
            _this.svg = "data:image/svg+xml," + encodeURIComponent(s.serializeToString(img));
            _this.intrinsicWidth = img.width.baseVal.value;
            _this.intrinsicHeight = img.height.baseVal.value;
            _this.context.cache.addImage(_this.svg);
            return _this;
        }
        return SVGElementContainer;
    }(ElementContainer));

    var LIElementContainer = /** @class */ (function (_super) {
        __extends(LIElementContainer, _super);
        function LIElementContainer(context, element) {
            var _this = _super.call(this, context, element) || this;
            _this.value = element.value;
            return _this;
        }
        return LIElementContainer;
    }(ElementContainer));

    var OLElementContainer = /** @class */ (function (_super) {
        __extends(OLElementContainer, _super);
        function OLElementContainer(context, element) {
            var _this = _super.call(this, context, element) || this;
            _this.start = element.start;
            _this.reversed = typeof element.reversed === 'boolean' && element.reversed === true;
            return _this;
        }
        return OLElementContainer;
    }(ElementContainer));

    var CHECKBOX_BORDER_RADIUS = [
        {
            type: 15 /* DIMENSION_TOKEN */,
            flags: 0,
            unit: 'px',
            number: 3
        }
    ];
    var RADIO_BORDER_RADIUS = [
        {
            type: 16 /* PERCENTAGE_TOKEN */,
            flags: 0,
            number: 50
        }
    ];
    var reformatInputBounds = function (bounds) {
        if (bounds.width > bounds.height) {
            return new Bounds(bounds.left + (bounds.width - bounds.height) / 2, bounds.top, bounds.height, bounds.height);
        }
        else if (bounds.width < bounds.height) {
            return new Bounds(bounds.left, bounds.top + (bounds.height - bounds.width) / 2, bounds.width, bounds.width);
        }
        return bounds;
    };
    var getInputValue = function (node) {
        var value = node.type === PASSWORD ? new Array(node.value.length + 1).join('\u2022') : node.value;
        return value.length === 0 ? node.placeholder || '' : value;
    };
    var CHECKBOX = 'checkbox';
    var RADIO = 'radio';
    var PASSWORD = 'password';
    var INPUT_COLOR = 0x2a2a2aff;
    var InputElementContainer = /** @class */ (function (_super) {
        __extends(InputElementContainer, _super);
        function InputElementContainer(context, input) {
            var _this = _super.call(this, context, input) || this;
            _this.type = input.type.toLowerCase();
            _this.checked = input.checked;
            _this.value = getInputValue(input);
            if (_this.type === CHECKBOX || _this.type === RADIO) {
                _this.styles.backgroundColor = 0xdededeff;
                _this.styles.borderTopColor =
                    _this.styles.borderRightColor =
                        _this.styles.borderBottomColor =
                            _this.styles.borderLeftColor =
                                0xa5a5a5ff;
                _this.styles.borderTopWidth =
                    _this.styles.borderRightWidth =
                        _this.styles.borderBottomWidth =
                            _this.styles.borderLeftWidth =
                                1;
                _this.styles.borderTopStyle =
                    _this.styles.borderRightStyle =
                        _this.styles.borderBottomStyle =
                            _this.styles.borderLeftStyle =
                                1 /* SOLID */;
                _this.styles.backgroundClip = [0 /* BORDER_BOX */];
                _this.styles.backgroundOrigin = [0 /* BORDER_BOX */];
                _this.bounds = reformatInputBounds(_this.bounds);
            }
            switch (_this.type) {
                case CHECKBOX:
                    _this.styles.borderTopRightRadius =
                        _this.styles.borderTopLeftRadius =
                            _this.styles.borderBottomRightRadius =
                                _this.styles.borderBottomLeftRadius =
                                    CHECKBOX_BORDER_RADIUS;
                    break;
                case RADIO:
                    _this.styles.borderTopRightRadius =
                        _this.styles.borderTopLeftRadius =
                            _this.styles.borderBottomRightRadius =
                                _this.styles.borderBottomLeftRadius =
                                    RADIO_BORDER_RADIUS;
                    break;
            }
            return _this;
        }
        return InputElementContainer;
    }(ElementContainer));

    var SelectElementContainer = /** @class */ (function (_super) {
        __extends(SelectElementContainer, _super);
        function SelectElementContainer(context, element) {
            var _this = _super.call(this, context, element) || this;
            var option = element.options[element.selectedIndex || 0];
            _this.value = option ? option.text || '' : '';
            return _this;
        }
        return SelectElementContainer;
    }(ElementContainer));

    var TextareaElementContainer = /** @class */ (function (_super) {
        __extends(TextareaElementContainer, _super);
        function TextareaElementContainer(context, element) {
            var _this = _super.call(this, context, element) || this;
            _this.value = element.value;
            return _this;
        }
        return TextareaElementContainer;
    }(ElementContainer));

    var IFrameElementContainer = /** @class */ (function (_super) {
        __extends(IFrameElementContainer, _super);
        function IFrameElementContainer(context, iframe) {
            var _this = _super.call(this, context, iframe) || this;
            _this.src = iframe.src;
            _this.width = parseInt(iframe.width, 10) || 0;
            _this.height = parseInt(iframe.height, 10) || 0;
            _this.backgroundColor = _this.styles.backgroundColor;
            try {
                if (iframe.contentWindow &&
                    iframe.contentWindow.document &&
                    iframe.contentWindow.document.documentElement) {
                    _this.tree = parseTree(context, iframe.contentWindow.document.documentElement);
                    // http://www.w3.org/TR/css3-background/#special-backgrounds
                    var documentBackgroundColor = iframe.contentWindow.document.documentElement
                        ? parseColor(context, getComputedStyle(iframe.contentWindow.document.documentElement).backgroundColor)
                        : COLORS.TRANSPARENT;
                    var bodyBackgroundColor = iframe.contentWindow.document.body
                        ? parseColor(context, getComputedStyle(iframe.contentWindow.document.body).backgroundColor)
                        : COLORS.TRANSPARENT;
                    _this.backgroundColor = isTransparent(documentBackgroundColor)
                        ? isTransparent(bodyBackgroundColor)
                            ? _this.styles.backgroundColor
                            : bodyBackgroundColor
                        : documentBackgroundColor;
                }
            }
            catch (e) { }
            return _this;
        }
        return IFrameElementContainer;
    }(ElementContainer));

    var LIST_OWNERS = ['OL', 'UL', 'MENU'];
    var parseNodeTree = function (context, node, parent, root) {
        for (var childNode = node.firstChild, nextNode = void 0; childNode; childNode = nextNode) {
            nextNode = childNode.nextSibling;
            if (isTextNode(childNode) && childNode.data.trim().length > 0) {
                parent.textNodes.push(new TextContainer(context, childNode, parent.styles));
            }
            else if (isElementNode(childNode)) {
                if (isSlotElement(childNode) && childNode.assignedNodes) {
                    childNode.assignedNodes().forEach(function (childNode) { return parseNodeTree(context, childNode, parent, root); });
                }
                else {
                    var container = createContainer(context, childNode);
                    if (container.styles.isVisible()) {
                        if (createsRealStackingContext(childNode, container, root)) {
                            container.flags |= 4 /* CREATES_REAL_STACKING_CONTEXT */;
                        }
                        else if (createsStackingContext(container.styles)) {
                            container.flags |= 2 /* CREATES_STACKING_CONTEXT */;
                        }
                        if (LIST_OWNERS.indexOf(childNode.tagName) !== -1) {
                            container.flags |= 8 /* IS_LIST_OWNER */;
                        }
                        parent.elements.push(container);
                        childNode.slot;
                        if (childNode.shadowRoot) {
                            parseNodeTree(context, childNode.shadowRoot, container, root);
                        }
                        else if (!isTextareaElement(childNode) &&
                            !isSVGElement(childNode) &&
                            !isSelectElement(childNode)) {
                            parseNodeTree(context, childNode, container, root);
                        }
                    }
                }
            }
        }
    };
    var createContainer = function (context, element) {
        if (isImageElement(element)) {
            return new ImageElementContainer(context, element);
        }
        if (isCanvasElement(element)) {
            return new CanvasElementContainer(context, element);
        }
        if (isSVGElement(element)) {
            return new SVGElementContainer(context, element);
        }
        if (isLIElement(element)) {
            return new LIElementContainer(context, element);
        }
        if (isOLElement(element)) {
            return new OLElementContainer(context, element);
        }
        if (isInputElement(element)) {
            return new InputElementContainer(context, element);
        }
        if (isSelectElement(element)) {
            return new SelectElementContainer(context, element);
        }
        if (isTextareaElement(element)) {
            return new TextareaElementContainer(context, element);
        }
        if (isIFrameElement(element)) {
            return new IFrameElementContainer(context, element);
        }
        return new ElementContainer(context, element);
    };
    var parseTree = function (context, element) {
        var container = createContainer(context, element);
        container.flags |= 4 /* CREATES_REAL_STACKING_CONTEXT */;
        parseNodeTree(context, element, container, container);
        return container;
    };
    var createsRealStackingContext = function (node, container, root) {
        return (container.styles.isPositionedWithZIndex() ||
            container.styles.opacity < 1 ||
            container.styles.isTransformed() ||
            (isBodyElement(node) && root.styles.isTransparent()));
    };
    var createsStackingContext = function (styles) { return styles.isPositioned() || styles.isFloating(); };
    var isTextNode = function (node) { return node.nodeType === Node.TEXT_NODE; };
    var isElementNode = function (node) { return node.nodeType === Node.ELEMENT_NODE; };
    var isHTMLElementNode = function (node) {
        return isElementNode(node) && typeof node.style !== 'undefined' && !isSVGElementNode(node);
    };
    var isSVGElementNode = function (element) {
        return typeof element.className === 'object';
    };
    var isLIElement = function (node) { return node.tagName === 'LI'; };
    var isOLElement = function (node) { return node.tagName === 'OL'; };
    var isInputElement = function (node) { return node.tagName === 'INPUT'; };
    var isHTMLElement = function (node) { return node.tagName === 'HTML'; };
    var isSVGElement = function (node) { return node.tagName === 'svg'; };
    var isBodyElement = function (node) { return node.tagName === 'BODY'; };
    var isCanvasElement = function (node) { return node.tagName === 'CANVAS'; };
    var isVideoElement = function (node) { return node.tagName === 'VIDEO'; };
    var isImageElement = function (node) { return node.tagName === 'IMG'; };
    var isIFrameElement = function (node) { return node.tagName === 'IFRAME'; };
    var isStyleElement = function (node) { return node.tagName === 'STYLE'; };
    var isScriptElement = function (node) { return node.tagName === 'SCRIPT'; };
    var isTextareaElement = function (node) { return node.tagName === 'TEXTAREA'; };
    var isSelectElement = function (node) { return node.tagName === 'SELECT'; };
    var isSlotElement = function (node) { return node.tagName === 'SLOT'; };
    // https://html.spec.whatwg.org/multipage/custom-elements.html#valid-custom-element-name
    var isCustomElement = function (node) { return node.tagName.indexOf('-') > 0; };

    var CounterState = /** @class */ (function () {
        function CounterState() {
            this.counters = {};
        }
        CounterState.prototype.getCounterValue = function (name) {
            var counter = this.counters[name];
            if (counter && counter.length) {
                return counter[counter.length - 1];
            }
            return 1;
        };
        CounterState.prototype.getCounterValues = function (name) {
            var counter = this.counters[name];
            return counter ? counter : [];
        };
        CounterState.prototype.pop = function (counters) {
            var _this = this;
            counters.forEach(function (counter) { return _this.counters[counter].pop(); });
        };
        CounterState.prototype.parse = function (style) {
            var _this = this;
            var counterIncrement = style.counterIncrement;
            var counterReset = style.counterReset;
            var canReset = true;
            if (counterIncrement !== null) {
                counterIncrement.forEach(function (entry) {
                    var counter = _this.counters[entry.counter];
                    if (counter && entry.increment !== 0) {
                        canReset = false;
                        if (!counter.length) {
                            counter.push(1);
                        }
                        counter[Math.max(0, counter.length - 1)] += entry.increment;
                    }
                });
            }
            var counterNames = [];
            if (canReset) {
                counterReset.forEach(function (entry) {
                    var counter = _this.counters[entry.counter];
                    counterNames.push(entry.counter);
                    if (!counter) {
                        counter = _this.counters[entry.counter] = [];
                    }
                    counter.push(entry.reset);
                });
            }
            return counterNames;
        };
        return CounterState;
    }());
    var ROMAN_UPPER = {
        integers: [1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1],
        values: ['M', 'CM', 'D', 'CD', 'C', 'XC', 'L', 'XL', 'X', 'IX', 'V', 'IV', 'I']
    };
    var ARMENIAN = {
        integers: [
            9000, 8000, 7000, 6000, 5000, 4000, 3000, 2000, 1000, 900, 800, 700, 600, 500, 400, 300, 200, 100, 90, 80, 70,
            60, 50, 40, 30, 20, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1
        ],
        values: [
            'Ք',
            'Փ',
            'Ւ',
            'Ց',
            'Ր',
            'Տ',
            'Վ',
            'Ս',
            'Ռ',
            'Ջ',
            'Պ',
            'Չ',
            'Ո',
            'Շ',
            'Ն',
            'Յ',
            'Մ',
            'Ճ',
            'Ղ',
            'Ձ',
            'Հ',
            'Կ',
            'Ծ',
            'Խ',
            'Լ',
            'Ի',
            'Ժ',
            'Թ',
            'Ը',
            'Է',
            'Զ',
            'Ե',
            'Դ',
            'Գ',
            'Բ',
            'Ա'
        ]
    };
    var HEBREW = {
        integers: [
            10000, 9000, 8000, 7000, 6000, 5000, 4000, 3000, 2000, 1000, 400, 300, 200, 100, 90, 80, 70, 60, 50, 40, 30, 20,
            19, 18, 17, 16, 15, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1
        ],
        values: [
            'י׳',
            'ט׳',
            'ח׳',
            'ז׳',
            'ו׳',
            'ה׳',
            'ד׳',
            'ג׳',
            'ב׳',
            'א׳',
            'ת',
            'ש',
            'ר',
            'ק',
            'צ',
            'פ',
            'ע',
            'ס',
            'נ',
            'מ',
            'ל',
            'כ',
            'יט',
            'יח',
            'יז',
            'טז',
            'טו',
            'י',
            'ט',
            'ח',
            'ז',
            'ו',
            'ה',
            'ד',
            'ג',
            'ב',
            'א'
        ]
    };
    var GEORGIAN = {
        integers: [
            10000, 9000, 8000, 7000, 6000, 5000, 4000, 3000, 2000, 1000, 900, 800, 700, 600, 500, 400, 300, 200, 100, 90,
            80, 70, 60, 50, 40, 30, 20, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1
        ],
        values: [
            'ჵ',
            'ჰ',
            'ჯ',
            'ჴ',
            'ხ',
            'ჭ',
            'წ',
            'ძ',
            'ც',
            'ჩ',
            'შ',
            'ყ',
            'ღ',
            'ქ',
            'ფ',
            'ჳ',
            'ტ',
            'ს',
            'რ',
            'ჟ',
            'პ',
            'ო',
            'ჲ',
            'ნ',
            'მ',
            'ლ',
            'კ',
            'ი',
            'თ',
            'ჱ',
            'ზ',
            'ვ',
            'ე',
            'დ',
            'გ',
            'ბ',
            'ა'
        ]
    };
    var createAdditiveCounter = function (value, min, max, symbols, fallback, suffix) {
        if (value < min || value > max) {
            return createCounterText(value, fallback, suffix.length > 0);
        }
        return (symbols.integers.reduce(function (string, integer, index) {
            while (value >= integer) {
                value -= integer;
                string += symbols.values[index];
            }
            return string;
        }, '') + suffix);
    };
    var createCounterStyleWithSymbolResolver = function (value, codePointRangeLength, isNumeric, resolver) {
        var string = '';
        do {
            if (!isNumeric) {
                value--;
            }
            string = resolver(value) + string;
            value /= codePointRangeLength;
        } while (value * codePointRangeLength >= codePointRangeLength);
        return string;
    };
    var createCounterStyleFromRange = function (value, codePointRangeStart, codePointRangeEnd, isNumeric, suffix) {
        var codePointRangeLength = codePointRangeEnd - codePointRangeStart + 1;
        return ((value < 0 ? '-' : '') +
            (createCounterStyleWithSymbolResolver(Math.abs(value), codePointRangeLength, isNumeric, function (codePoint) {
                return fromCodePoint$1(Math.floor(codePoint % codePointRangeLength) + codePointRangeStart);
            }) +
                suffix));
    };
    var createCounterStyleFromSymbols = function (value, symbols, suffix) {
        if (suffix === void 0) { suffix = '. '; }
        var codePointRangeLength = symbols.length;
        return (createCounterStyleWithSymbolResolver(Math.abs(value), codePointRangeLength, false, function (codePoint) { return symbols[Math.floor(codePoint % codePointRangeLength)]; }) + suffix);
    };
    var CJK_ZEROS = 1 << 0;
    var CJK_TEN_COEFFICIENTS = 1 << 1;
    var CJK_TEN_HIGH_COEFFICIENTS = 1 << 2;
    var CJK_HUNDRED_COEFFICIENTS = 1 << 3;
    var createCJKCounter = function (value, numbers, multipliers, negativeSign, suffix, flags) {
        if (value < -9999 || value > 9999) {
            return createCounterText(value, 4 /* CJK_DECIMAL */, suffix.length > 0);
        }
        var tmp = Math.abs(value);
        var string = suffix;
        if (tmp === 0) {
            return numbers[0] + string;
        }
        for (var digit = 0; tmp > 0 && digit <= 4; digit++) {
            var coefficient = tmp % 10;
            if (coefficient === 0 && contains(flags, CJK_ZEROS) && string !== '') {
                string = numbers[coefficient] + string;
            }
            else if (coefficient > 1 ||
                (coefficient === 1 && digit === 0) ||
                (coefficient === 1 && digit === 1 && contains(flags, CJK_TEN_COEFFICIENTS)) ||
                (coefficient === 1 && digit === 1 && contains(flags, CJK_TEN_HIGH_COEFFICIENTS) && value > 100) ||
                (coefficient === 1 && digit > 1 && contains(flags, CJK_HUNDRED_COEFFICIENTS))) {
                string = numbers[coefficient] + (digit > 0 ? multipliers[digit - 1] : '') + string;
            }
            else if (coefficient === 1 && digit > 0) {
                string = multipliers[digit - 1] + string;
            }
            tmp = Math.floor(tmp / 10);
        }
        return (value < 0 ? negativeSign : '') + string;
    };
    var CHINESE_INFORMAL_MULTIPLIERS = '十百千萬';
    var CHINESE_FORMAL_MULTIPLIERS = '拾佰仟萬';
    var JAPANESE_NEGATIVE = 'マイナス';
    var KOREAN_NEGATIVE = '마이너스';
    var createCounterText = function (value, type, appendSuffix) {
        var defaultSuffix = appendSuffix ? '. ' : '';
        var cjkSuffix = appendSuffix ? '、' : '';
        var koreanSuffix = appendSuffix ? ', ' : '';
        var spaceSuffix = appendSuffix ? ' ' : '';
        switch (type) {
            case 0 /* DISC */:
                return '•' + spaceSuffix;
            case 1 /* CIRCLE */:
                return '◦' + spaceSuffix;
            case 2 /* SQUARE */:
                return '◾' + spaceSuffix;
            case 5 /* DECIMAL_LEADING_ZERO */:
                var string = createCounterStyleFromRange(value, 48, 57, true, defaultSuffix);
                return string.length < 4 ? "0" + string : string;
            case 4 /* CJK_DECIMAL */:
                return createCounterStyleFromSymbols(value, '〇一二三四五六七八九', cjkSuffix);
            case 6 /* LOWER_ROMAN */:
                return createAdditiveCounter(value, 1, 3999, ROMAN_UPPER, 3 /* DECIMAL */, defaultSuffix).toLowerCase();
            case 7 /* UPPER_ROMAN */:
                return createAdditiveCounter(value, 1, 3999, ROMAN_UPPER, 3 /* DECIMAL */, defaultSuffix);
            case 8 /* LOWER_GREEK */:
                return createCounterStyleFromRange(value, 945, 969, false, defaultSuffix);
            case 9 /* LOWER_ALPHA */:
                return createCounterStyleFromRange(value, 97, 122, false, defaultSuffix);
            case 10 /* UPPER_ALPHA */:
                return createCounterStyleFromRange(value, 65, 90, false, defaultSuffix);
            case 11 /* ARABIC_INDIC */:
                return createCounterStyleFromRange(value, 1632, 1641, true, defaultSuffix);
            case 12 /* ARMENIAN */:
            case 49 /* UPPER_ARMENIAN */:
                return createAdditiveCounter(value, 1, 9999, ARMENIAN, 3 /* DECIMAL */, defaultSuffix);
            case 35 /* LOWER_ARMENIAN */:
                return createAdditiveCounter(value, 1, 9999, ARMENIAN, 3 /* DECIMAL */, defaultSuffix).toLowerCase();
            case 13 /* BENGALI */:
                return createCounterStyleFromRange(value, 2534, 2543, true, defaultSuffix);
            case 14 /* CAMBODIAN */:
            case 30 /* KHMER */:
                return createCounterStyleFromRange(value, 6112, 6121, true, defaultSuffix);
            case 15 /* CJK_EARTHLY_BRANCH */:
                return createCounterStyleFromSymbols(value, '子丑寅卯辰巳午未申酉戌亥', cjkSuffix);
            case 16 /* CJK_HEAVENLY_STEM */:
                return createCounterStyleFromSymbols(value, '甲乙丙丁戊己庚辛壬癸', cjkSuffix);
            case 17 /* CJK_IDEOGRAPHIC */:
            case 48 /* TRAD_CHINESE_INFORMAL */:
                return createCJKCounter(value, '零一二三四五六七八九', CHINESE_INFORMAL_MULTIPLIERS, '負', cjkSuffix, CJK_TEN_COEFFICIENTS | CJK_TEN_HIGH_COEFFICIENTS | CJK_HUNDRED_COEFFICIENTS);
            case 47 /* TRAD_CHINESE_FORMAL */:
                return createCJKCounter(value, '零壹貳參肆伍陸柒捌玖', CHINESE_FORMAL_MULTIPLIERS, '負', cjkSuffix, CJK_ZEROS | CJK_TEN_COEFFICIENTS | CJK_TEN_HIGH_COEFFICIENTS | CJK_HUNDRED_COEFFICIENTS);
            case 42 /* SIMP_CHINESE_INFORMAL */:
                return createCJKCounter(value, '零一二三四五六七八九', CHINESE_INFORMAL_MULTIPLIERS, '负', cjkSuffix, CJK_TEN_COEFFICIENTS | CJK_TEN_HIGH_COEFFICIENTS | CJK_HUNDRED_COEFFICIENTS);
            case 41 /* SIMP_CHINESE_FORMAL */:
                return createCJKCounter(value, '零壹贰叁肆伍陆柒捌玖', CHINESE_FORMAL_MULTIPLIERS, '负', cjkSuffix, CJK_ZEROS | CJK_TEN_COEFFICIENTS | CJK_TEN_HIGH_COEFFICIENTS | CJK_HUNDRED_COEFFICIENTS);
            case 26 /* JAPANESE_INFORMAL */:
                return createCJKCounter(value, '〇一二三四五六七八九', '十百千万', JAPANESE_NEGATIVE, cjkSuffix, 0);
            case 25 /* JAPANESE_FORMAL */:
                return createCJKCounter(value, '零壱弐参四伍六七八九', '拾百千万', JAPANESE_NEGATIVE, cjkSuffix, CJK_ZEROS | CJK_TEN_COEFFICIENTS | CJK_TEN_HIGH_COEFFICIENTS);
            case 31 /* KOREAN_HANGUL_FORMAL */:
                return createCJKCounter(value, '영일이삼사오육칠팔구', '십백천만', KOREAN_NEGATIVE, koreanSuffix, CJK_ZEROS | CJK_TEN_COEFFICIENTS | CJK_TEN_HIGH_COEFFICIENTS);
            case 33 /* KOREAN_HANJA_INFORMAL */:
                return createCJKCounter(value, '零一二三四五六七八九', '十百千萬', KOREAN_NEGATIVE, koreanSuffix, 0);
            case 32 /* KOREAN_HANJA_FORMAL */:
                return createCJKCounter(value, '零壹貳參四五六七八九', '拾百千', KOREAN_NEGATIVE, koreanSuffix, CJK_ZEROS | CJK_TEN_COEFFICIENTS | CJK_TEN_HIGH_COEFFICIENTS);
            case 18 /* DEVANAGARI */:
                return createCounterStyleFromRange(value, 0x966, 0x96f, true, defaultSuffix);
            case 20 /* GEORGIAN */:
                return createAdditiveCounter(value, 1, 19999, GEORGIAN, 3 /* DECIMAL */, defaultSuffix);
            case 21 /* GUJARATI */:
                return createCounterStyleFromRange(value, 0xae6, 0xaef, true, defaultSuffix);
            case 22 /* GURMUKHI */:
                return createCounterStyleFromRange(value, 0xa66, 0xa6f, true, defaultSuffix);
            case 22 /* HEBREW */:
                return createAdditiveCounter(value, 1, 10999, HEBREW, 3 /* DECIMAL */, defaultSuffix);
            case 23 /* HIRAGANA */:
                return createCounterStyleFromSymbols(value, 'あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわゐゑをん');
            case 24 /* HIRAGANA_IROHA */:
                return createCounterStyleFromSymbols(value, 'いろはにほへとちりぬるをわかよたれそつねならむうゐのおくやまけふこえてあさきゆめみしゑひもせす');
            case 27 /* KANNADA */:
                return createCounterStyleFromRange(value, 0xce6, 0xcef, true, defaultSuffix);
            case 28 /* KATAKANA */:
                return createCounterStyleFromSymbols(value, 'アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヰヱヲン', cjkSuffix);
            case 29 /* KATAKANA_IROHA */:
                return createCounterStyleFromSymbols(value, 'イロハニホヘトチリヌルヲワカヨタレソツネナラムウヰノオクヤマケフコエテアサキユメミシヱヒモセス', cjkSuffix);
            case 34 /* LAO */:
                return createCounterStyleFromRange(value, 0xed0, 0xed9, true, defaultSuffix);
            case 37 /* MONGOLIAN */:
                return createCounterStyleFromRange(value, 0x1810, 0x1819, true, defaultSuffix);
            case 38 /* MYANMAR */:
                return createCounterStyleFromRange(value, 0x1040, 0x1049, true, defaultSuffix);
            case 39 /* ORIYA */:
                return createCounterStyleFromRange(value, 0xb66, 0xb6f, true, defaultSuffix);
            case 40 /* PERSIAN */:
                return createCounterStyleFromRange(value, 0x6f0, 0x6f9, true, defaultSuffix);
            case 43 /* TAMIL */:
                return createCounterStyleFromRange(value, 0xbe6, 0xbef, true, defaultSuffix);
            case 44 /* TELUGU */:
                return createCounterStyleFromRange(value, 0xc66, 0xc6f, true, defaultSuffix);
            case 45 /* THAI */:
                return createCounterStyleFromRange(value, 0xe50, 0xe59, true, defaultSuffix);
            case 46 /* TIBETAN */:
                return createCounterStyleFromRange(value, 0xf20, 0xf29, true, defaultSuffix);
            case 3 /* DECIMAL */:
            default:
                return createCounterStyleFromRange(value, 48, 57, true, defaultSuffix);
        }
    };

    var IGNORE_ATTRIBUTE = 'data-html2canvas-ignore';
    var DocumentCloner = /** @class */ (function () {
        function DocumentCloner(context, element, options) {
            this.context = context;
            this.options = options;
            this.scrolledElements = [];
            this.referenceElement = element;
            this.counters = new CounterState();
            this.quoteDepth = 0;
            if (!element.ownerDocument) {
                throw new Error('Cloned element does not have an owner document');
            }
            this.documentElement = this.cloneNode(element.ownerDocument.documentElement, false);
        }
        DocumentCloner.prototype.toIFrame = function (ownerDocument, windowSize) {
            var _this = this;
            var iframe = createIFrameContainer(ownerDocument, windowSize);
            if (!iframe.contentWindow) {
                return Promise.reject("Unable to find iframe window");
            }
            var scrollX = ownerDocument.defaultView.pageXOffset;
            var scrollY = ownerDocument.defaultView.pageYOffset;
            var cloneWindow = iframe.contentWindow;
            var documentClone = cloneWindow.document;
            /* Chrome doesn't detect relative background-images assigned in inline <style> sheets when fetched through getComputedStyle
             if window url is about:blank, we can assign the url to current by writing onto the document
             */
            var iframeLoad = iframeLoader(iframe).then(function () { return __awaiter(_this, void 0, void 0, function () {
                var onclone, referenceElement;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            this.scrolledElements.forEach(restoreNodeScroll);
                            if (cloneWindow) {
                                cloneWindow.scrollTo(windowSize.left, windowSize.top);
                                if (/(iPad|iPhone|iPod)/g.test(navigator.userAgent) &&
                                    (cloneWindow.scrollY !== windowSize.top || cloneWindow.scrollX !== windowSize.left)) {
                                    this.context.logger.warn('Unable to restore scroll position for cloned document');
                                    this.context.windowBounds = this.context.windowBounds.add(cloneWindow.scrollX - windowSize.left, cloneWindow.scrollY - windowSize.top, 0, 0);
                                }
                            }
                            onclone = this.options.onclone;
                            referenceElement = this.clonedReferenceElement;
                            if (typeof referenceElement === 'undefined') {
                                return [2 /*return*/, Promise.reject("Error finding the " + this.referenceElement.nodeName + " in the cloned document")];
                            }
                            if (!(documentClone.fonts && documentClone.fonts.ready)) return [3 /*break*/, 2];
                            return [4 /*yield*/, documentClone.fonts.ready];
                        case 1:
                            _a.sent();
                            _a.label = 2;
                        case 2:
                            if (!/(AppleWebKit)/g.test(navigator.userAgent)) return [3 /*break*/, 4];
                            return [4 /*yield*/, imagesReady(documentClone)];
                        case 3:
                            _a.sent();
                            _a.label = 4;
                        case 4:
                            if (typeof onclone === 'function') {
                                return [2 /*return*/, Promise.resolve()
                                        .then(function () { return onclone(documentClone, referenceElement); })
                                        .then(function () { return iframe; })];
                            }
                            return [2 /*return*/, iframe];
                    }
                });
            }); });
            documentClone.open();
            documentClone.write(serializeDoctype(document.doctype) + "<html></html>");
            // Chrome scrolls the parent document for some reason after the write to the cloned window???
            restoreOwnerScroll(this.referenceElement.ownerDocument, scrollX, scrollY);
            documentClone.replaceChild(documentClone.adoptNode(this.documentElement), documentClone.documentElement);
            documentClone.close();
            return iframeLoad;
        };
        DocumentCloner.prototype.createElementClone = function (node) {
            if (isDebugging(node, 2 /* CLONE */)) {
                debugger;
            }
            if (isCanvasElement(node)) {
                return this.createCanvasClone(node);
            }
            if (isVideoElement(node)) {
                return this.createVideoClone(node);
            }
            if (isStyleElement(node)) {
                return this.createStyleClone(node);
            }
            var clone = node.cloneNode(false);
            if (isImageElement(clone)) {
                if (isImageElement(node) && node.currentSrc && node.currentSrc !== node.src) {
                    clone.src = node.currentSrc;
                    clone.srcset = '';
                }
                if (clone.loading === 'lazy') {
                    clone.loading = 'eager';
                }
            }
            if (isCustomElement(clone)) {
                return this.createCustomElementClone(clone);
            }
            return clone;
        };
        DocumentCloner.prototype.createCustomElementClone = function (node) {
            var clone = document.createElement('html2canvascustomelement');
            copyCSSStyles(node.style, clone);
            return clone;
        };
        DocumentCloner.prototype.createStyleClone = function (node) {
            try {
                var sheet = node.sheet;
                if (sheet && sheet.cssRules) {
                    var css = [].slice.call(sheet.cssRules, 0).reduce(function (css, rule) {
                        if (rule && typeof rule.cssText === 'string') {
                            return css + rule.cssText;
                        }
                        return css;
                    }, '');
                    var style = node.cloneNode(false);
                    style.textContent = css;
                    return style;
                }
            }
            catch (e) {
                // accessing node.sheet.cssRules throws a DOMException
                this.context.logger.error('Unable to access cssRules property', e);
                if (e.name !== 'SecurityError') {
                    throw e;
                }
            }
            return node.cloneNode(false);
        };
        DocumentCloner.prototype.createCanvasClone = function (canvas) {
            var _a;
            if (this.options.inlineImages && canvas.ownerDocument) {
                var img = canvas.ownerDocument.createElement('img');
                try {
                    img.src = canvas.toDataURL();
                    return img;
                }
                catch (e) {
                    this.context.logger.info("Unable to inline canvas contents, canvas is tainted", canvas);
                }
            }
            var clonedCanvas = canvas.cloneNode(false);
            try {
                clonedCanvas.width = canvas.width;
                clonedCanvas.height = canvas.height;
                var ctx = canvas.getContext('2d');
                var clonedCtx = clonedCanvas.getContext('2d');
                if (clonedCtx) {
                    if (!this.options.allowTaint && ctx) {
                        clonedCtx.putImageData(ctx.getImageData(0, 0, canvas.width, canvas.height), 0, 0);
                    }
                    else {
                        var gl = (_a = canvas.getContext('webgl2')) !== null && _a !== void 0 ? _a : canvas.getContext('webgl');
                        if (gl) {
                            var attribs = gl.getContextAttributes();
                            if ((attribs === null || attribs === void 0 ? void 0 : attribs.preserveDrawingBuffer) === false) {
                                this.context.logger.warn('Unable to clone WebGL context as it has preserveDrawingBuffer=false', canvas);
                            }
                        }
                        clonedCtx.drawImage(canvas, 0, 0);
                    }
                }
                return clonedCanvas;
            }
            catch (e) {
                this.context.logger.info("Unable to clone canvas as it is tainted", canvas);
            }
            return clonedCanvas;
        };
        DocumentCloner.prototype.createVideoClone = function (video) {
            var canvas = video.ownerDocument.createElement('canvas');
            canvas.width = video.offsetWidth;
            canvas.height = video.offsetHeight;
            var ctx = canvas.getContext('2d');
            try {
                if (ctx) {
                    ctx.drawImage(video, 0, 0, canvas.width, canvas.height);
                    if (!this.options.allowTaint) {
                        ctx.getImageData(0, 0, canvas.width, canvas.height);
                    }
                }
                return canvas;
            }
            catch (e) {
                this.context.logger.info("Unable to clone video as it is tainted", video);
            }
            var blankCanvas = video.ownerDocument.createElement('canvas');
            blankCanvas.width = video.offsetWidth;
            blankCanvas.height = video.offsetHeight;
            return blankCanvas;
        };
        DocumentCloner.prototype.appendChildNode = function (clone, child, copyStyles) {
            if (!isElementNode(child) ||
                (!isScriptElement(child) &&
                    !child.hasAttribute(IGNORE_ATTRIBUTE) &&
                    (typeof this.options.ignoreElements !== 'function' || !this.options.ignoreElements(child)))) {
                if (!this.options.copyStyles || !isElementNode(child) || !isStyleElement(child)) {
                    clone.appendChild(this.cloneNode(child, copyStyles));
                }
            }
        };
        DocumentCloner.prototype.cloneChildNodes = function (node, clone, copyStyles) {
            var _this = this;
            for (var child = node.shadowRoot ? node.shadowRoot.firstChild : node.firstChild; child; child = child.nextSibling) {
                if (isElementNode(child) && isSlotElement(child) && typeof child.assignedNodes === 'function') {
                    var assignedNodes = child.assignedNodes();
                    if (assignedNodes.length) {
                        assignedNodes.forEach(function (assignedNode) { return _this.appendChildNode(clone, assignedNode, copyStyles); });
                    }
                }
                else {
                    this.appendChildNode(clone, child, copyStyles);
                }
            }
        };
        DocumentCloner.prototype.cloneNode = function (node, copyStyles) {
            if (isTextNode(node)) {
                return document.createTextNode(node.data);
            }
            if (!node.ownerDocument) {
                return node.cloneNode(false);
            }
            var window = node.ownerDocument.defaultView;
            if (window && isElementNode(node) && (isHTMLElementNode(node) || isSVGElementNode(node))) {
                var clone = this.createElementClone(node);
                clone.style.transitionProperty = 'none';
                var style = window.getComputedStyle(node);
                var styleBefore = window.getComputedStyle(node, ':before');
                var styleAfter = window.getComputedStyle(node, ':after');
                if (this.referenceElement === node && isHTMLElementNode(clone)) {
                    this.clonedReferenceElement = clone;
                }
                if (isBodyElement(clone)) {
                    createPseudoHideStyles(clone);
                }
                var counters = this.counters.parse(new CSSParsedCounterDeclaration(this.context, style));
                var before = this.resolvePseudoContent(node, clone, styleBefore, PseudoElementType.BEFORE);
                if (isCustomElement(node)) {
                    copyStyles = true;
                }
                if (!isVideoElement(node)) {
                    this.cloneChildNodes(node, clone, copyStyles);
                }
                if (before) {
                    clone.insertBefore(before, clone.firstChild);
                }
                var after = this.resolvePseudoContent(node, clone, styleAfter, PseudoElementType.AFTER);
                if (after) {
                    clone.appendChild(after);
                }
                this.counters.pop(counters);
                if ((style && (this.options.copyStyles || isSVGElementNode(node)) && !isIFrameElement(node)) ||
                    copyStyles) {
                    copyCSSStyles(style, clone);
                }
                if (node.scrollTop !== 0 || node.scrollLeft !== 0) {
                    this.scrolledElements.push([clone, node.scrollLeft, node.scrollTop]);
                }
                if ((isTextareaElement(node) || isSelectElement(node)) &&
                    (isTextareaElement(clone) || isSelectElement(clone))) {
                    clone.value = node.value;
                }
                return clone;
            }
            return node.cloneNode(false);
        };
        DocumentCloner.prototype.resolvePseudoContent = function (node, clone, style, pseudoElt) {
            var _this = this;
            if (!style) {
                return;
            }
            var value = style.content;
            var document = clone.ownerDocument;
            if (!document || !value || value === 'none' || value === '-moz-alt-content' || style.display === 'none') {
                return;
            }
            this.counters.parse(new CSSParsedCounterDeclaration(this.context, style));
            var declaration = new CSSParsedPseudoDeclaration(this.context, style);
            var anonymousReplacedElement = document.createElement('html2canvaspseudoelement');
            copyCSSStyles(style, anonymousReplacedElement);
            declaration.content.forEach(function (token) {
                if (token.type === 0 /* STRING_TOKEN */) {
                    anonymousReplacedElement.appendChild(document.createTextNode(token.value));
                }
                else if (token.type === 22 /* URL_TOKEN */) {
                    var img = document.createElement('img');
                    img.src = token.value;
                    img.style.opacity = '1';
                    anonymousReplacedElement.appendChild(img);
                }
                else if (token.type === 18 /* FUNCTION */) {
                    if (token.name === 'attr') {
                        var attr = token.values.filter(isIdentToken);
                        if (attr.length) {
                            anonymousReplacedElement.appendChild(document.createTextNode(node.getAttribute(attr[0].value) || ''));
                        }
                    }
                    else if (token.name === 'counter') {
                        var _a = token.values.filter(nonFunctionArgSeparator), counter = _a[0], counterStyle = _a[1];
                        if (counter && isIdentToken(counter)) {
                            var counterState = _this.counters.getCounterValue(counter.value);
                            var counterType = counterStyle && isIdentToken(counterStyle)
                                ? listStyleType.parse(_this.context, counterStyle.value)
                                : 3 /* DECIMAL */;
                            anonymousReplacedElement.appendChild(document.createTextNode(createCounterText(counterState, counterType, false)));
                        }
                    }
                    else if (token.name === 'counters') {
                        var _b = token.values.filter(nonFunctionArgSeparator), counter = _b[0], delim = _b[1], counterStyle = _b[2];
                        if (counter && isIdentToken(counter)) {
                            var counterStates = _this.counters.getCounterValues(counter.value);
                            var counterType_1 = counterStyle && isIdentToken(counterStyle)
                                ? listStyleType.parse(_this.context, counterStyle.value)
                                : 3 /* DECIMAL */;
                            var separator = delim && delim.type === 0 /* STRING_TOKEN */ ? delim.value : '';
                            var text = counterStates
                                .map(function (value) { return createCounterText(value, counterType_1, false); })
                                .join(separator);
                            anonymousReplacedElement.appendChild(document.createTextNode(text));
                        }
                    }
                    else ;
                }
                else if (token.type === 20 /* IDENT_TOKEN */) {
                    switch (token.value) {
                        case 'open-quote':
                            anonymousReplacedElement.appendChild(document.createTextNode(getQuote(declaration.quotes, _this.quoteDepth++, true)));
                            break;
                        case 'close-quote':
                            anonymousReplacedElement.appendChild(document.createTextNode(getQuote(declaration.quotes, --_this.quoteDepth, false)));
                            break;
                        default:
                            // safari doesn't parse string tokens correctly because of lack of quotes
                            anonymousReplacedElement.appendChild(document.createTextNode(token.value));
                    }
                }
            });
            anonymousReplacedElement.className = PSEUDO_HIDE_ELEMENT_CLASS_BEFORE + " " + PSEUDO_HIDE_ELEMENT_CLASS_AFTER;
            var newClassName = pseudoElt === PseudoElementType.BEFORE
                ? " " + PSEUDO_HIDE_ELEMENT_CLASS_BEFORE
                : " " + PSEUDO_HIDE_ELEMENT_CLASS_AFTER;
            if (isSVGElementNode(clone)) {
                clone.className.baseValue += newClassName;
            }
            else {
                clone.className += newClassName;
            }
            return anonymousReplacedElement;
        };
        DocumentCloner.destroy = function (container) {
            if (container.parentNode) {
                container.parentNode.removeChild(container);
                return true;
            }
            return false;
        };
        return DocumentCloner;
    }());
    var PseudoElementType;
    (function (PseudoElementType) {
        PseudoElementType[PseudoElementType["BEFORE"] = 0] = "BEFORE";
        PseudoElementType[PseudoElementType["AFTER"] = 1] = "AFTER";
    })(PseudoElementType || (PseudoElementType = {}));
    var createIFrameContainer = function (ownerDocument, bounds) {
        var cloneIframeContainer = ownerDocument.createElement('iframe');
        cloneIframeContainer.className = 'html2canvas-container';
        cloneIframeContainer.style.visibility = 'hidden';
        cloneIframeContainer.style.position = 'fixed';
        cloneIframeContainer.style.left = '-10000px';
        cloneIframeContainer.style.top = '0px';
        cloneIframeContainer.style.border = '0';
        cloneIframeContainer.width = bounds.width.toString();
        cloneIframeContainer.height = bounds.height.toString();
        cloneIframeContainer.scrolling = 'no'; // ios won't scroll without it
        cloneIframeContainer.setAttribute(IGNORE_ATTRIBUTE, 'true');
        ownerDocument.body.appendChild(cloneIframeContainer);
        return cloneIframeContainer;
    };
    var imageReady = function (img) {
        return new Promise(function (resolve) {
            if (img.complete) {
                resolve();
                return;
            }
            if (!img.src) {
                resolve();
                return;
            }
            img.onload = resolve;
            img.onerror = resolve;
        });
    };
    var imagesReady = function (document) {
        return Promise.all([].slice.call(document.images, 0).map(imageReady));
    };
    var iframeLoader = function (iframe) {
        return new Promise(function (resolve, reject) {
            var cloneWindow = iframe.contentWindow;
            if (!cloneWindow) {
                return reject("No window assigned for iframe");
            }
            var documentClone = cloneWindow.document;
            cloneWindow.onload = iframe.onload = function () {
                cloneWindow.onload = iframe.onload = null;
                var interval = setInterval(function () {
                    if (documentClone.body.childNodes.length > 0 && documentClone.readyState === 'complete') {
                        clearInterval(interval);
                        resolve(iframe);
                    }
                }, 50);
            };
        });
    };
    var ignoredStyleProperties = [
        'all',
        'd',
        'content' // Safari shows pseudoelements if content is set
    ];
    var copyCSSStyles = function (style, target) {
        // Edge does not provide value for cssText
        for (var i = style.length - 1; i >= 0; i--) {
            var property = style.item(i);
            if (ignoredStyleProperties.indexOf(property) === -1) {
                target.style.setProperty(property, style.getPropertyValue(property));
            }
        }
        return target;
    };
    var serializeDoctype = function (doctype) {
        var str = '';
        if (doctype) {
            str += '<!DOCTYPE ';
            if (doctype.name) {
                str += doctype.name;
            }
            if (doctype.internalSubset) {
                str += doctype.internalSubset;
            }
            if (doctype.publicId) {
                str += "\"" + doctype.publicId + "\"";
            }
            if (doctype.systemId) {
                str += "\"" + doctype.systemId + "\"";
            }
            str += '>';
        }
        return str;
    };
    var restoreOwnerScroll = function (ownerDocument, x, y) {
        if (ownerDocument &&
            ownerDocument.defaultView &&
            (x !== ownerDocument.defaultView.pageXOffset || y !== ownerDocument.defaultView.pageYOffset)) {
            ownerDocument.defaultView.scrollTo(x, y);
        }
    };
    var restoreNodeScroll = function (_a) {
        var element = _a[0], x = _a[1], y = _a[2];
        element.scrollLeft = x;
        element.scrollTop = y;
    };
    var PSEUDO_BEFORE = ':before';
    var PSEUDO_AFTER = ':after';
    var PSEUDO_HIDE_ELEMENT_CLASS_BEFORE = '___html2canvas___pseudoelement_before';
    var PSEUDO_HIDE_ELEMENT_CLASS_AFTER = '___html2canvas___pseudoelement_after';
    var PSEUDO_HIDE_ELEMENT_STYLE = "{\n    content: \"\" !important;\n    display: none !important;\n}";
    var createPseudoHideStyles = function (body) {
        createStyles(body, "." + PSEUDO_HIDE_ELEMENT_CLASS_BEFORE + PSEUDO_BEFORE + PSEUDO_HIDE_ELEMENT_STYLE + "\n         ." + PSEUDO_HIDE_ELEMENT_CLASS_AFTER + PSEUDO_AFTER + PSEUDO_HIDE_ELEMENT_STYLE);
    };
    var createStyles = function (body, styles) {
        var document = body.ownerDocument;
        if (document) {
            var style = document.createElement('style');
            style.textContent = styles;
            body.appendChild(style);
        }
    };

    var CacheStorage = /** @class */ (function () {
        function CacheStorage() {
        }
        CacheStorage.getOrigin = function (url) {
            var link = CacheStorage._link;
            if (!link) {
                return 'about:blank';
            }
            link.href = url;
            link.href = link.href; // IE9, LOL! - http://jsfiddle.net/niklasvh/2e48b/
            return link.protocol + link.hostname + link.port;
        };
        CacheStorage.isSameOrigin = function (src) {
            return CacheStorage.getOrigin(src) === CacheStorage._origin;
        };
        CacheStorage.setContext = function (window) {
            CacheStorage._link = window.document.createElement('a');
            CacheStorage._origin = CacheStorage.getOrigin(window.location.href);
        };
        CacheStorage._origin = 'about:blank';
        return CacheStorage;
    }());
    var Cache = /** @class */ (function () {
        function Cache(context, _options) {
            this.context = context;
            this._options = _options;
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            this._cache = {};
        }
        Cache.prototype.addImage = function (src) {
            var result = Promise.resolve();
            if (this.has(src)) {
                return result;
            }
            if (isBlobImage(src) || isRenderable(src)) {
                (this._cache[src] = this.loadImage(src)).catch(function () {
                    // prevent unhandled rejection
                });
                return result;
            }
            return result;
        };
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        Cache.prototype.match = function (src) {
            return this._cache[src];
        };
        Cache.prototype.loadImage = function (key) {
            return __awaiter(this, void 0, void 0, function () {
                var isSameOrigin, useCORS, useProxy, src;
                var _this = this;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            isSameOrigin = CacheStorage.isSameOrigin(key);
                            useCORS = !isInlineImage(key) && this._options.useCORS === true && FEATURES.SUPPORT_CORS_IMAGES && !isSameOrigin;
                            useProxy = !isInlineImage(key) &&
                                !isSameOrigin &&
                                !isBlobImage(key) &&
                                typeof this._options.proxy === 'string' &&
                                FEATURES.SUPPORT_CORS_XHR &&
                                !useCORS;
                            if (!isSameOrigin &&
                                this._options.allowTaint === false &&
                                !isInlineImage(key) &&
                                !isBlobImage(key) &&
                                !useProxy &&
                                !useCORS) {
                                return [2 /*return*/];
                            }
                            src = key;
                            if (!useProxy) return [3 /*break*/, 2];
                            return [4 /*yield*/, this.proxy(src)];
                        case 1:
                            src = _a.sent();
                            _a.label = 2;
                        case 2:
                            this.context.logger.debug("Added image " + key.substring(0, 256));
                            return [4 /*yield*/, new Promise(function (resolve, reject) {
                                    var img = new Image();
                                    img.onload = function () { return resolve(img); };
                                    img.onerror = reject;
                                    //ios safari 10.3 taints canvas with data urls unless crossOrigin is set to anonymous
                                    if (isInlineBase64Image(src) || useCORS) {
                                        img.crossOrigin = 'anonymous';
                                    }
                                    img.src = src;
                                    if (img.complete === true) {
                                        // Inline XML images may fail to parse, throwing an Error later on
                                        setTimeout(function () { return resolve(img); }, 500);
                                    }
                                    if (_this._options.imageTimeout > 0) {
                                        setTimeout(function () { return reject("Timed out (" + _this._options.imageTimeout + "ms) loading image"); }, _this._options.imageTimeout);
                                    }
                                })];
                        case 3: return [2 /*return*/, _a.sent()];
                    }
                });
            });
        };
        Cache.prototype.has = function (key) {
            return typeof this._cache[key] !== 'undefined';
        };
        Cache.prototype.keys = function () {
            return Promise.resolve(Object.keys(this._cache));
        };
        Cache.prototype.proxy = function (src) {
            var _this = this;
            var proxy = this._options.proxy;
            if (!proxy) {
                throw new Error('No proxy defined');
            }
            var key = src.substring(0, 256);
            return new Promise(function (resolve, reject) {
                var responseType = FEATURES.SUPPORT_RESPONSE_TYPE ? 'blob' : 'text';
                var xhr = new XMLHttpRequest();
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        if (responseType === 'text') {
                            resolve(xhr.response);
                        }
                        else {
                            var reader_1 = new FileReader();
                            reader_1.addEventListener('load', function () { return resolve(reader_1.result); }, false);
                            reader_1.addEventListener('error', function (e) { return reject(e); }, false);
                            reader_1.readAsDataURL(xhr.response);
                        }
                    }
                    else {
                        reject("Failed to proxy resource " + key + " with status code " + xhr.status);
                    }
                };
                xhr.onerror = reject;
                var queryString = proxy.indexOf('?') > -1 ? '&' : '?';
                xhr.open('GET', "" + proxy + queryString + "url=" + encodeURIComponent(src) + "&responseType=" + responseType);
                if (responseType !== 'text' && xhr instanceof XMLHttpRequest) {
                    xhr.responseType = responseType;
                }
                if (_this._options.imageTimeout) {
                    var timeout_1 = _this._options.imageTimeout;
                    xhr.timeout = timeout_1;
                    xhr.ontimeout = function () { return reject("Timed out (" + timeout_1 + "ms) proxying " + key); };
                }
                xhr.send();
            });
        };
        return Cache;
    }());
    var INLINE_SVG = /^data:image\/svg\+xml/i;
    var INLINE_BASE64 = /^data:image\/.*;base64,/i;
    var INLINE_IMG = /^data:image\/.*/i;
    var isRenderable = function (src) { return FEATURES.SUPPORT_SVG_DRAWING || !isSVG(src); };
    var isInlineImage = function (src) { return INLINE_IMG.test(src); };
    var isInlineBase64Image = function (src) { return INLINE_BASE64.test(src); };
    var isBlobImage = function (src) { return src.substr(0, 4) === 'blob'; };
    var isSVG = function (src) { return src.substr(-3).toLowerCase() === 'svg' || INLINE_SVG.test(src); };

    var Vector = /** @class */ (function () {
        function Vector(x, y) {
            this.type = 0 /* VECTOR */;
            this.x = x;
            this.y = y;
        }
        Vector.prototype.add = function (deltaX, deltaY) {
            return new Vector(this.x + deltaX, this.y + deltaY);
        };
        return Vector;
    }());

    var lerp = function (a, b, t) {
        return new Vector(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
    };
    var BezierCurve = /** @class */ (function () {
        function BezierCurve(start, startControl, endControl, end) {
            this.type = 1 /* BEZIER_CURVE */;
            this.start = start;
            this.startControl = startControl;
            this.endControl = endControl;
            this.end = end;
        }
        BezierCurve.prototype.subdivide = function (t, firstHalf) {
            var ab = lerp(this.start, this.startControl, t);
            var bc = lerp(this.startControl, this.endControl, t);
            var cd = lerp(this.endControl, this.end, t);
            var abbc = lerp(ab, bc, t);
            var bccd = lerp(bc, cd, t);
            var dest = lerp(abbc, bccd, t);
            return firstHalf ? new BezierCurve(this.start, ab, abbc, dest) : new BezierCurve(dest, bccd, cd, this.end);
        };
        BezierCurve.prototype.add = function (deltaX, deltaY) {
            return new BezierCurve(this.start.add(deltaX, deltaY), this.startControl.add(deltaX, deltaY), this.endControl.add(deltaX, deltaY), this.end.add(deltaX, deltaY));
        };
        BezierCurve.prototype.reverse = function () {
            return new BezierCurve(this.end, this.endControl, this.startControl, this.start);
        };
        return BezierCurve;
    }());
    var isBezierCurve = function (path) { return path.type === 1 /* BEZIER_CURVE */; };

    var BoundCurves = /** @class */ (function () {
        function BoundCurves(element) {
            var styles = element.styles;
            var bounds = element.bounds;
            var _a = getAbsoluteValueForTuple(styles.borderTopLeftRadius, bounds.width, bounds.height), tlh = _a[0], tlv = _a[1];
            var _b = getAbsoluteValueForTuple(styles.borderTopRightRadius, bounds.width, bounds.height), trh = _b[0], trv = _b[1];
            var _c = getAbsoluteValueForTuple(styles.borderBottomRightRadius, bounds.width, bounds.height), brh = _c[0], brv = _c[1];
            var _d = getAbsoluteValueForTuple(styles.borderBottomLeftRadius, bounds.width, bounds.height), blh = _d[0], blv = _d[1];
            var factors = [];
            factors.push((tlh + trh) / bounds.width);
            factors.push((blh + brh) / bounds.width);
            factors.push((tlv + blv) / bounds.height);
            factors.push((trv + brv) / bounds.height);
            var maxFactor = Math.max.apply(Math, factors);
            if (maxFactor > 1) {
                tlh /= maxFactor;
                tlv /= maxFactor;
                trh /= maxFactor;
                trv /= maxFactor;
                brh /= maxFactor;
                brv /= maxFactor;
                blh /= maxFactor;
                blv /= maxFactor;
            }
            var topWidth = bounds.width - trh;
            var rightHeight = bounds.height - brv;
            var bottomWidth = bounds.width - brh;
            var leftHeight = bounds.height - blv;
            var borderTopWidth = styles.borderTopWidth;
            var borderRightWidth = styles.borderRightWidth;
            var borderBottomWidth = styles.borderBottomWidth;
            var borderLeftWidth = styles.borderLeftWidth;
            var paddingTop = getAbsoluteValue(styles.paddingTop, element.bounds.width);
            var paddingRight = getAbsoluteValue(styles.paddingRight, element.bounds.width);
            var paddingBottom = getAbsoluteValue(styles.paddingBottom, element.bounds.width);
            var paddingLeft = getAbsoluteValue(styles.paddingLeft, element.bounds.width);
            this.topLeftBorderDoubleOuterBox =
                tlh > 0 || tlv > 0
                    ? getCurvePoints(bounds.left + borderLeftWidth / 3, bounds.top + borderTopWidth / 3, tlh - borderLeftWidth / 3, tlv - borderTopWidth / 3, CORNER.TOP_LEFT)
                    : new Vector(bounds.left + borderLeftWidth / 3, bounds.top + borderTopWidth / 3);
            this.topRightBorderDoubleOuterBox =
                tlh > 0 || tlv > 0
                    ? getCurvePoints(bounds.left + topWidth, bounds.top + borderTopWidth / 3, trh - borderRightWidth / 3, trv - borderTopWidth / 3, CORNER.TOP_RIGHT)
                    : new Vector(bounds.left + bounds.width - borderRightWidth / 3, bounds.top + borderTopWidth / 3);
            this.bottomRightBorderDoubleOuterBox =
                brh > 0 || brv > 0
                    ? getCurvePoints(bounds.left + bottomWidth, bounds.top + rightHeight, brh - borderRightWidth / 3, brv - borderBottomWidth / 3, CORNER.BOTTOM_RIGHT)
                    : new Vector(bounds.left + bounds.width - borderRightWidth / 3, bounds.top + bounds.height - borderBottomWidth / 3);
            this.bottomLeftBorderDoubleOuterBox =
                blh > 0 || blv > 0
                    ? getCurvePoints(bounds.left + borderLeftWidth / 3, bounds.top + leftHeight, blh - borderLeftWidth / 3, blv - borderBottomWidth / 3, CORNER.BOTTOM_LEFT)
                    : new Vector(bounds.left + borderLeftWidth / 3, bounds.top + bounds.height - borderBottomWidth / 3);
            this.topLeftBorderDoubleInnerBox =
                tlh > 0 || tlv > 0
                    ? getCurvePoints(bounds.left + (borderLeftWidth * 2) / 3, bounds.top + (borderTopWidth * 2) / 3, tlh - (borderLeftWidth * 2) / 3, tlv - (borderTopWidth * 2) / 3, CORNER.TOP_LEFT)
                    : new Vector(bounds.left + (borderLeftWidth * 2) / 3, bounds.top + (borderTopWidth * 2) / 3);
            this.topRightBorderDoubleInnerBox =
                tlh > 0 || tlv > 0
                    ? getCurvePoints(bounds.left + topWidth, bounds.top + (borderTopWidth * 2) / 3, trh - (borderRightWidth * 2) / 3, trv - (borderTopWidth * 2) / 3, CORNER.TOP_RIGHT)
                    : new Vector(bounds.left + bounds.width - (borderRightWidth * 2) / 3, bounds.top + (borderTopWidth * 2) / 3);
            this.bottomRightBorderDoubleInnerBox =
                brh > 0 || brv > 0
                    ? getCurvePoints(bounds.left + bottomWidth, bounds.top + rightHeight, brh - (borderRightWidth * 2) / 3, brv - (borderBottomWidth * 2) / 3, CORNER.BOTTOM_RIGHT)
                    : new Vector(bounds.left + bounds.width - (borderRightWidth * 2) / 3, bounds.top + bounds.height - (borderBottomWidth * 2) / 3);
            this.bottomLeftBorderDoubleInnerBox =
                blh > 0 || blv > 0
                    ? getCurvePoints(bounds.left + (borderLeftWidth * 2) / 3, bounds.top + leftHeight, blh - (borderLeftWidth * 2) / 3, blv - (borderBottomWidth * 2) / 3, CORNER.BOTTOM_LEFT)
                    : new Vector(bounds.left + (borderLeftWidth * 2) / 3, bounds.top + bounds.height - (borderBottomWidth * 2) / 3);
            this.topLeftBorderStroke =
                tlh > 0 || tlv > 0
                    ? getCurvePoints(bounds.left + borderLeftWidth / 2, bounds.top + borderTopWidth / 2, tlh - borderLeftWidth / 2, tlv - borderTopWidth / 2, CORNER.TOP_LEFT)
                    : new Vector(bounds.left + borderLeftWidth / 2, bounds.top + borderTopWidth / 2);
            this.topRightBorderStroke =
                tlh > 0 || tlv > 0
                    ? getCurvePoints(bounds.left + topWidth, bounds.top + borderTopWidth / 2, trh - borderRightWidth / 2, trv - borderTopWidth / 2, CORNER.TOP_RIGHT)
                    : new Vector(bounds.left + bounds.width - borderRightWidth / 2, bounds.top + borderTopWidth / 2);
            this.bottomRightBorderStroke =
                brh > 0 || brv > 0
                    ? getCurvePoints(bounds.left + bottomWidth, bounds.top + rightHeight, brh - borderRightWidth / 2, brv - borderBottomWidth / 2, CORNER.BOTTOM_RIGHT)
                    : new Vector(bounds.left + bounds.width - borderRightWidth / 2, bounds.top + bounds.height - borderBottomWidth / 2);
            this.bottomLeftBorderStroke =
                blh > 0 || blv > 0
                    ? getCurvePoints(bounds.left + borderLeftWidth / 2, bounds.top + leftHeight, blh - borderLeftWidth / 2, blv - borderBottomWidth / 2, CORNER.BOTTOM_LEFT)
                    : new Vector(bounds.left + borderLeftWidth / 2, bounds.top + bounds.height - borderBottomWidth / 2);
            this.topLeftBorderBox =
                tlh > 0 || tlv > 0
                    ? getCurvePoints(bounds.left, bounds.top, tlh, tlv, CORNER.TOP_LEFT)
                    : new Vector(bounds.left, bounds.top);
            this.topRightBorderBox =
                trh > 0 || trv > 0
                    ? getCurvePoints(bounds.left + topWidth, bounds.top, trh, trv, CORNER.TOP_RIGHT)
                    : new Vector(bounds.left + bounds.width, bounds.top);
            this.bottomRightBorderBox =
                brh > 0 || brv > 0
                    ? getCurvePoints(bounds.left + bottomWidth, bounds.top + rightHeight, brh, brv, CORNER.BOTTOM_RIGHT)
                    : new Vector(bounds.left + bounds.width, bounds.top + bounds.height);
            this.bottomLeftBorderBox =
                blh > 0 || blv > 0
                    ? getCurvePoints(bounds.left, bounds.top + leftHeight, blh, blv, CORNER.BOTTOM_LEFT)
                    : new Vector(bounds.left, bounds.top + bounds.height);
            this.topLeftPaddingBox =
                tlh > 0 || tlv > 0
                    ? getCurvePoints(bounds.left + borderLeftWidth, bounds.top + borderTopWidth, Math.max(0, tlh - borderLeftWidth), Math.max(0, tlv - borderTopWidth), CORNER.TOP_LEFT)
                    : new Vector(bounds.left + borderLeftWidth, bounds.top + borderTopWidth);
            this.topRightPaddingBox =
                trh > 0 || trv > 0
                    ? getCurvePoints(bounds.left + Math.min(topWidth, bounds.width - borderRightWidth), bounds.top + borderTopWidth, topWidth > bounds.width + borderRightWidth ? 0 : Math.max(0, trh - borderRightWidth), Math.max(0, trv - borderTopWidth), CORNER.TOP_RIGHT)
                    : new Vector(bounds.left + bounds.width - borderRightWidth, bounds.top + borderTopWidth);
            this.bottomRightPaddingBox =
                brh > 0 || brv > 0
                    ? getCurvePoints(bounds.left + Math.min(bottomWidth, bounds.width - borderLeftWidth), bounds.top + Math.min(rightHeight, bounds.height - borderBottomWidth), Math.max(0, brh - borderRightWidth), Math.max(0, brv - borderBottomWidth), CORNER.BOTTOM_RIGHT)
                    : new Vector(bounds.left + bounds.width - borderRightWidth, bounds.top + bounds.height - borderBottomWidth);
            this.bottomLeftPaddingBox =
                blh > 0 || blv > 0
                    ? getCurvePoints(bounds.left + borderLeftWidth, bounds.top + Math.min(leftHeight, bounds.height - borderBottomWidth), Math.max(0, blh - borderLeftWidth), Math.max(0, blv - borderBottomWidth), CORNER.BOTTOM_LEFT)
                    : new Vector(bounds.left + borderLeftWidth, bounds.top + bounds.height - borderBottomWidth);
            this.topLeftContentBox =
                tlh > 0 || tlv > 0
                    ? getCurvePoints(bounds.left + borderLeftWidth + paddingLeft, bounds.top + borderTopWidth + paddingTop, Math.max(0, tlh - (borderLeftWidth + paddingLeft)), Math.max(0, tlv - (borderTopWidth + paddingTop)), CORNER.TOP_LEFT)
                    : new Vector(bounds.left + borderLeftWidth + paddingLeft, bounds.top + borderTopWidth + paddingTop);
            this.topRightContentBox =
                trh > 0 || trv > 0
                    ? getCurvePoints(bounds.left + Math.min(topWidth, bounds.width + borderLeftWidth + paddingLeft), bounds.top + borderTopWidth + paddingTop, topWidth > bounds.width + borderLeftWidth + paddingLeft ? 0 : trh - borderLeftWidth + paddingLeft, trv - (borderTopWidth + paddingTop), CORNER.TOP_RIGHT)
                    : new Vector(bounds.left + bounds.width - (borderRightWidth + paddingRight), bounds.top + borderTopWidth + paddingTop);
            this.bottomRightContentBox =
                brh > 0 || brv > 0
                    ? getCurvePoints(bounds.left + Math.min(bottomWidth, bounds.width - (borderLeftWidth + paddingLeft)), bounds.top + Math.min(rightHeight, bounds.height + borderTopWidth + paddingTop), Math.max(0, brh - (borderRightWidth + paddingRight)), brv - (borderBottomWidth + paddingBottom), CORNER.BOTTOM_RIGHT)
                    : new Vector(bounds.left + bounds.width - (borderRightWidth + paddingRight), bounds.top + bounds.height - (borderBottomWidth + paddingBottom));
            this.bottomLeftContentBox =
                blh > 0 || blv > 0
                    ? getCurvePoints(bounds.left + borderLeftWidth + paddingLeft, bounds.top + leftHeight, Math.max(0, blh - (borderLeftWidth + paddingLeft)), blv - (borderBottomWidth + paddingBottom), CORNER.BOTTOM_LEFT)
                    : new Vector(bounds.left + borderLeftWidth + paddingLeft, bounds.top + bounds.height - (borderBottomWidth + paddingBottom));
        }
        return BoundCurves;
    }());
    var CORNER;
    (function (CORNER) {
        CORNER[CORNER["TOP_LEFT"] = 0] = "TOP_LEFT";
        CORNER[CORNER["TOP_RIGHT"] = 1] = "TOP_RIGHT";
        CORNER[CORNER["BOTTOM_RIGHT"] = 2] = "BOTTOM_RIGHT";
        CORNER[CORNER["BOTTOM_LEFT"] = 3] = "BOTTOM_LEFT";
    })(CORNER || (CORNER = {}));
    var getCurvePoints = function (x, y, r1, r2, position) {
        var kappa = 4 * ((Math.sqrt(2) - 1) / 3);
        var ox = r1 * kappa; // control point offset horizontal
        var oy = r2 * kappa; // control point offset vertical
        var xm = x + r1; // x-middle
        var ym = y + r2; // y-middle
        switch (position) {
            case CORNER.TOP_LEFT:
                return new BezierCurve(new Vector(x, ym), new Vector(x, ym - oy), new Vector(xm - ox, y), new Vector(xm, y));
            case CORNER.TOP_RIGHT:
                return new BezierCurve(new Vector(x, y), new Vector(x + ox, y), new Vector(xm, ym - oy), new Vector(xm, ym));
            case CORNER.BOTTOM_RIGHT:
                return new BezierCurve(new Vector(xm, y), new Vector(xm, y + oy), new Vector(x + ox, ym), new Vector(x, ym));
            case CORNER.BOTTOM_LEFT:
            default:
                return new BezierCurve(new Vector(xm, ym), new Vector(xm - ox, ym), new Vector(x, y + oy), new Vector(x, y));
        }
    };
    var calculateBorderBoxPath = function (curves) {
        return [curves.topLeftBorderBox, curves.topRightBorderBox, curves.bottomRightBorderBox, curves.bottomLeftBorderBox];
    };
    var calculateContentBoxPath = function (curves) {
        return [
            curves.topLeftContentBox,
            curves.topRightContentBox,
            curves.bottomRightContentBox,
            curves.bottomLeftContentBox
        ];
    };
    var calculatePaddingBoxPath = function (curves) {
        return [
            curves.topLeftPaddingBox,
            curves.topRightPaddingBox,
            curves.bottomRightPaddingBox,
            curves.bottomLeftPaddingBox
        ];
    };

    var TransformEffect = /** @class */ (function () {
        function TransformEffect(offsetX, offsetY, matrix) {
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.matrix = matrix;
            this.type = 0 /* TRANSFORM */;
            this.target = 2 /* BACKGROUND_BORDERS */ | 4 /* CONTENT */;
        }
        return TransformEffect;
    }());
    var ClipEffect = /** @class */ (function () {
        function ClipEffect(path, target) {
            this.path = path;
            this.target = target;
            this.type = 1 /* CLIP */;
        }
        return ClipEffect;
    }());
    var OpacityEffect = /** @class */ (function () {
        function OpacityEffect(opacity) {
            this.opacity = opacity;
            this.type = 2 /* OPACITY */;
            this.target = 2 /* BACKGROUND_BORDERS */ | 4 /* CONTENT */;
        }
        return OpacityEffect;
    }());
    var isTransformEffect = function (effect) {
        return effect.type === 0 /* TRANSFORM */;
    };
    var isClipEffect = function (effect) { return effect.type === 1 /* CLIP */; };
    var isOpacityEffect = function (effect) { return effect.type === 2 /* OPACITY */; };

    var equalPath = function (a, b) {
        if (a.length === b.length) {
            return a.some(function (v, i) { return v === b[i]; });
        }
        return false;
    };
    var transformPath = function (path, deltaX, deltaY, deltaW, deltaH) {
        return path.map(function (point, index) {
            switch (index) {
                case 0:
                    return point.add(deltaX, deltaY);
                case 1:
                    return point.add(deltaX + deltaW, deltaY);
                case 2:
                    return point.add(deltaX + deltaW, deltaY + deltaH);
                case 3:
                    return point.add(deltaX, deltaY + deltaH);
            }
            return point;
        });
    };

    var StackingContext = /** @class */ (function () {
        function StackingContext(container) {
            this.element = container;
            this.inlineLevel = [];
            this.nonInlineLevel = [];
            this.negativeZIndex = [];
            this.zeroOrAutoZIndexOrTransformedOrOpacity = [];
            this.positiveZIndex = [];
            this.nonPositionedFloats = [];
            this.nonPositionedInlineLevel = [];
        }
        return StackingContext;
    }());
    var ElementPaint = /** @class */ (function () {
        function ElementPaint(container, parent) {
            this.container = container;
            this.parent = parent;
            this.effects = [];
            this.curves = new BoundCurves(this.container);
            if (this.container.styles.opacity < 1) {
                this.effects.push(new OpacityEffect(this.container.styles.opacity));
            }
            if (this.container.styles.transform !== null) {
                var offsetX = this.container.bounds.left + this.container.styles.transformOrigin[0].number;
                var offsetY = this.container.bounds.top + this.container.styles.transformOrigin[1].number;
                var matrix = this.container.styles.transform;
                this.effects.push(new TransformEffect(offsetX, offsetY, matrix));
            }
            if (this.container.styles.overflowX !== 0 /* VISIBLE */) {
                var borderBox = calculateBorderBoxPath(this.curves);
                var paddingBox = calculatePaddingBoxPath(this.curves);
                if (equalPath(borderBox, paddingBox)) {
                    this.effects.push(new ClipEffect(borderBox, 2 /* BACKGROUND_BORDERS */ | 4 /* CONTENT */));
                }
                else {
                    this.effects.push(new ClipEffect(borderBox, 2 /* BACKGROUND_BORDERS */));
                    this.effects.push(new ClipEffect(paddingBox, 4 /* CONTENT */));
                }
            }
        }
        ElementPaint.prototype.getEffects = function (target) {
            var inFlow = [2 /* ABSOLUTE */, 3 /* FIXED */].indexOf(this.container.styles.position) === -1;
            var parent = this.parent;
            var effects = this.effects.slice(0);
            while (parent) {
                var croplessEffects = parent.effects.filter(function (effect) { return !isClipEffect(effect); });
                if (inFlow || parent.container.styles.position !== 0 /* STATIC */ || !parent.parent) {
                    effects.unshift.apply(effects, croplessEffects);
                    inFlow = [2 /* ABSOLUTE */, 3 /* FIXED */].indexOf(parent.container.styles.position) === -1;
                    if (parent.container.styles.overflowX !== 0 /* VISIBLE */) {
                        var borderBox = calculateBorderBoxPath(parent.curves);
                        var paddingBox = calculatePaddingBoxPath(parent.curves);
                        if (!equalPath(borderBox, paddingBox)) {
                            effects.unshift(new ClipEffect(paddingBox, 2 /* BACKGROUND_BORDERS */ | 4 /* CONTENT */));
                        }
                    }
                }
                else {
                    effects.unshift.apply(effects, croplessEffects);
                }
                parent = parent.parent;
            }
            return effects.filter(function (effect) { return contains(effect.target, target); });
        };
        return ElementPaint;
    }());
    var parseStackTree = function (parent, stackingContext, realStackingContext, listItems) {
        parent.container.elements.forEach(function (child) {
            var treatAsRealStackingContext = contains(child.flags, 4 /* CREATES_REAL_STACKING_CONTEXT */);
            var createsStackingContext = contains(child.flags, 2 /* CREATES_STACKING_CONTEXT */);
            var paintContainer = new ElementPaint(child, parent);
            if (contains(child.styles.display, 2048 /* LIST_ITEM */)) {
                listItems.push(paintContainer);
            }
            var listOwnerItems = contains(child.flags, 8 /* IS_LIST_OWNER */) ? [] : listItems;
            if (treatAsRealStackingContext || createsStackingContext) {
                var parentStack = treatAsRealStackingContext || child.styles.isPositioned() ? realStackingContext : stackingContext;
                var stack = new StackingContext(paintContainer);
                if (child.styles.isPositioned() || child.styles.opacity < 1 || child.styles.isTransformed()) {
                    var order_1 = child.styles.zIndex.order;
                    if (order_1 < 0) {
                        var index_1 = 0;
                        parentStack.negativeZIndex.some(function (current, i) {
                            if (order_1 > current.element.container.styles.zIndex.order) {
                                index_1 = i;
                                return false;
                            }
                            else if (index_1 > 0) {
                                return true;
                            }
                            return false;
                        });
                        parentStack.negativeZIndex.splice(index_1, 0, stack);
                    }
                    else if (order_1 > 0) {
                        var index_2 = 0;
                        parentStack.positiveZIndex.some(function (current, i) {
                            if (order_1 >= current.element.container.styles.zIndex.order) {
                                index_2 = i + 1;
                                return false;
                            }
                            else if (index_2 > 0) {
                                return true;
                            }
                            return false;
                        });
                        parentStack.positiveZIndex.splice(index_2, 0, stack);
                    }
                    else {
                        parentStack.zeroOrAutoZIndexOrTransformedOrOpacity.push(stack);
                    }
                }
                else {
                    if (child.styles.isFloating()) {
                        parentStack.nonPositionedFloats.push(stack);
                    }
                    else {
                        parentStack.nonPositionedInlineLevel.push(stack);
                    }
                }
                parseStackTree(paintContainer, stack, treatAsRealStackingContext ? stack : realStackingContext, listOwnerItems);
            }
            else {
                if (child.styles.isInlineLevel()) {
                    stackingContext.inlineLevel.push(paintContainer);
                }
                else {
                    stackingContext.nonInlineLevel.push(paintContainer);
                }
                parseStackTree(paintContainer, stackingContext, realStackingContext, listOwnerItems);
            }
            if (contains(child.flags, 8 /* IS_LIST_OWNER */)) {
                processListItems(child, listOwnerItems);
            }
        });
    };
    var processListItems = function (owner, elements) {
        var numbering = owner instanceof OLElementContainer ? owner.start : 1;
        var reversed = owner instanceof OLElementContainer ? owner.reversed : false;
        for (var i = 0; i < elements.length; i++) {
            var item = elements[i];
            if (item.container instanceof LIElementContainer &&
                typeof item.container.value === 'number' &&
                item.container.value !== 0) {
                numbering = item.container.value;
            }
            item.listValue = createCounterText(numbering, item.container.styles.listStyleType, true);
            numbering += reversed ? -1 : 1;
        }
    };
    var parseStackingContexts = function (container) {
        var paintContainer = new ElementPaint(container, null);
        var root = new StackingContext(paintContainer);
        var listItems = [];
        parseStackTree(paintContainer, root, root, listItems);
        processListItems(paintContainer.container, listItems);
        return root;
    };

    var parsePathForBorder = function (curves, borderSide) {
        switch (borderSide) {
            case 0:
                return createPathFromCurves(curves.topLeftBorderBox, curves.topLeftPaddingBox, curves.topRightBorderBox, curves.topRightPaddingBox);
            case 1:
                return createPathFromCurves(curves.topRightBorderBox, curves.topRightPaddingBox, curves.bottomRightBorderBox, curves.bottomRightPaddingBox);
            case 2:
                return createPathFromCurves(curves.bottomRightBorderBox, curves.bottomRightPaddingBox, curves.bottomLeftBorderBox, curves.bottomLeftPaddingBox);
            case 3:
            default:
                return createPathFromCurves(curves.bottomLeftBorderBox, curves.bottomLeftPaddingBox, curves.topLeftBorderBox, curves.topLeftPaddingBox);
        }
    };
    var parsePathForBorderDoubleOuter = function (curves, borderSide) {
        switch (borderSide) {
            case 0:
                return createPathFromCurves(curves.topLeftBorderBox, curves.topLeftBorderDoubleOuterBox, curves.topRightBorderBox, curves.topRightBorderDoubleOuterBox);
            case 1:
                return createPathFromCurves(curves.topRightBorderBox, curves.topRightBorderDoubleOuterBox, curves.bottomRightBorderBox, curves.bottomRightBorderDoubleOuterBox);
            case 2:
                return createPathFromCurves(curves.bottomRightBorderBox, curves.bottomRightBorderDoubleOuterBox, curves.bottomLeftBorderBox, curves.bottomLeftBorderDoubleOuterBox);
            case 3:
            default:
                return createPathFromCurves(curves.bottomLeftBorderBox, curves.bottomLeftBorderDoubleOuterBox, curves.topLeftBorderBox, curves.topLeftBorderDoubleOuterBox);
        }
    };
    var parsePathForBorderDoubleInner = function (curves, borderSide) {
        switch (borderSide) {
            case 0:
                return createPathFromCurves(curves.topLeftBorderDoubleInnerBox, curves.topLeftPaddingBox, curves.topRightBorderDoubleInnerBox, curves.topRightPaddingBox);
            case 1:
                return createPathFromCurves(curves.topRightBorderDoubleInnerBox, curves.topRightPaddingBox, curves.bottomRightBorderDoubleInnerBox, curves.bottomRightPaddingBox);
            case 2:
                return createPathFromCurves(curves.bottomRightBorderDoubleInnerBox, curves.bottomRightPaddingBox, curves.bottomLeftBorderDoubleInnerBox, curves.bottomLeftPaddingBox);
            case 3:
            default:
                return createPathFromCurves(curves.bottomLeftBorderDoubleInnerBox, curves.bottomLeftPaddingBox, curves.topLeftBorderDoubleInnerBox, curves.topLeftPaddingBox);
        }
    };
    var parsePathForBorderStroke = function (curves, borderSide) {
        switch (borderSide) {
            case 0:
                return createStrokePathFromCurves(curves.topLeftBorderStroke, curves.topRightBorderStroke);
            case 1:
                return createStrokePathFromCurves(curves.topRightBorderStroke, curves.bottomRightBorderStroke);
            case 2:
                return createStrokePathFromCurves(curves.bottomRightBorderStroke, curves.bottomLeftBorderStroke);
            case 3:
            default:
                return createStrokePathFromCurves(curves.bottomLeftBorderStroke, curves.topLeftBorderStroke);
        }
    };
    var createStrokePathFromCurves = function (outer1, outer2) {
        var path = [];
        if (isBezierCurve(outer1)) {
            path.push(outer1.subdivide(0.5, false));
        }
        else {
            path.push(outer1);
        }
        if (isBezierCurve(outer2)) {
            path.push(outer2.subdivide(0.5, true));
        }
        else {
            path.push(outer2);
        }
        return path;
    };
    var createPathFromCurves = function (outer1, inner1, outer2, inner2) {
        var path = [];
        if (isBezierCurve(outer1)) {
            path.push(outer1.subdivide(0.5, false));
        }
        else {
            path.push(outer1);
        }
        if (isBezierCurve(outer2)) {
            path.push(outer2.subdivide(0.5, true));
        }
        else {
            path.push(outer2);
        }
        if (isBezierCurve(inner2)) {
            path.push(inner2.subdivide(0.5, true).reverse());
        }
        else {
            path.push(inner2);
        }
        if (isBezierCurve(inner1)) {
            path.push(inner1.subdivide(0.5, false).reverse());
        }
        else {
            path.push(inner1);
        }
        return path;
    };

    var paddingBox = function (element) {
        var bounds = element.bounds;
        var styles = element.styles;
        return bounds.add(styles.borderLeftWidth, styles.borderTopWidth, -(styles.borderRightWidth + styles.borderLeftWidth), -(styles.borderTopWidth + styles.borderBottomWidth));
    };
    var contentBox = function (element) {
        var styles = element.styles;
        var bounds = element.bounds;
        var paddingLeft = getAbsoluteValue(styles.paddingLeft, bounds.width);
        var paddingRight = getAbsoluteValue(styles.paddingRight, bounds.width);
        var paddingTop = getAbsoluteValue(styles.paddingTop, bounds.width);
        var paddingBottom = getAbsoluteValue(styles.paddingBottom, bounds.width);
        return bounds.add(paddingLeft + styles.borderLeftWidth, paddingTop + styles.borderTopWidth, -(styles.borderRightWidth + styles.borderLeftWidth + paddingLeft + paddingRight), -(styles.borderTopWidth + styles.borderBottomWidth + paddingTop + paddingBottom));
    };

    var calculateBackgroundPositioningArea = function (backgroundOrigin, element) {
        if (backgroundOrigin === 0 /* BORDER_BOX */) {
            return element.bounds;
        }
        if (backgroundOrigin === 2 /* CONTENT_BOX */) {
            return contentBox(element);
        }
        return paddingBox(element);
    };
    var calculateBackgroundPaintingArea = function (backgroundClip, element) {
        if (backgroundClip === 0 /* BORDER_BOX */) {
            return element.bounds;
        }
        if (backgroundClip === 2 /* CONTENT_BOX */) {
            return contentBox(element);
        }
        return paddingBox(element);
    };
    var calculateBackgroundRendering = function (container, index, intrinsicSize) {
        var backgroundPositioningArea = calculateBackgroundPositioningArea(getBackgroundValueForIndex(container.styles.backgroundOrigin, index), container);
        var backgroundPaintingArea = calculateBackgroundPaintingArea(getBackgroundValueForIndex(container.styles.backgroundClip, index), container);
        var backgroundImageSize = calculateBackgroundSize(getBackgroundValueForIndex(container.styles.backgroundSize, index), intrinsicSize, backgroundPositioningArea);
        var sizeWidth = backgroundImageSize[0], sizeHeight = backgroundImageSize[1];
        var position = getAbsoluteValueForTuple(getBackgroundValueForIndex(container.styles.backgroundPosition, index), backgroundPositioningArea.width - sizeWidth, backgroundPositioningArea.height - sizeHeight);
        var path = calculateBackgroundRepeatPath(getBackgroundValueForIndex(container.styles.backgroundRepeat, index), position, backgroundImageSize, backgroundPositioningArea, backgroundPaintingArea);
        var offsetX = Math.round(backgroundPositioningArea.left + position[0]);
        var offsetY = Math.round(backgroundPositioningArea.top + position[1]);
        return [path, offsetX, offsetY, sizeWidth, sizeHeight];
    };
    var isAuto = function (token) { return isIdentToken(token) && token.value === BACKGROUND_SIZE.AUTO; };
    var hasIntrinsicValue = function (value) { return typeof value === 'number'; };
    var calculateBackgroundSize = function (size, _a, bounds) {
        var intrinsicWidth = _a[0], intrinsicHeight = _a[1], intrinsicProportion = _a[2];
        var first = size[0], second = size[1];
        if (!first) {
            return [0, 0];
        }
        if (isLengthPercentage(first) && second && isLengthPercentage(second)) {
            return [getAbsoluteValue(first, bounds.width), getAbsoluteValue(second, bounds.height)];
        }
        var hasIntrinsicProportion = hasIntrinsicValue(intrinsicProportion);
        if (isIdentToken(first) && (first.value === BACKGROUND_SIZE.CONTAIN || first.value === BACKGROUND_SIZE.COVER)) {
            if (hasIntrinsicValue(intrinsicProportion)) {
                var targetRatio = bounds.width / bounds.height;
                return targetRatio < intrinsicProportion !== (first.value === BACKGROUND_SIZE.COVER)
                    ? [bounds.width, bounds.width / intrinsicProportion]
                    : [bounds.height * intrinsicProportion, bounds.height];
            }
            return [bounds.width, bounds.height];
        }
        var hasIntrinsicWidth = hasIntrinsicValue(intrinsicWidth);
        var hasIntrinsicHeight = hasIntrinsicValue(intrinsicHeight);
        var hasIntrinsicDimensions = hasIntrinsicWidth || hasIntrinsicHeight;
        // If the background-size is auto or auto auto:
        if (isAuto(first) && (!second || isAuto(second))) {
            // If the image has both horizontal and vertical intrinsic dimensions, it's rendered at that size.
            if (hasIntrinsicWidth && hasIntrinsicHeight) {
                return [intrinsicWidth, intrinsicHeight];
            }
            // If the image has no intrinsic dimensions and has no intrinsic proportions,
            // it's rendered at the size of the background positioning area.
            if (!hasIntrinsicProportion && !hasIntrinsicDimensions) {
                return [bounds.width, bounds.height];
            }
            // TODO If the image has no intrinsic dimensions but has intrinsic proportions, it's rendered as if contain had been specified instead.
            // If the image has only one intrinsic dimension and has intrinsic proportions, it's rendered at the size corresponding to that one dimension.
            // The other dimension is computed using the specified dimension and the intrinsic proportions.
            if (hasIntrinsicDimensions && hasIntrinsicProportion) {
                var width_1 = hasIntrinsicWidth
                    ? intrinsicWidth
                    : intrinsicHeight * intrinsicProportion;
                var height_1 = hasIntrinsicHeight
                    ? intrinsicHeight
                    : intrinsicWidth / intrinsicProportion;
                return [width_1, height_1];
            }
            // If the image has only one intrinsic dimension but has no intrinsic proportions,
            // it's rendered using the specified dimension and the other dimension of the background positioning area.
            var width_2 = hasIntrinsicWidth ? intrinsicWidth : bounds.width;
            var height_2 = hasIntrinsicHeight ? intrinsicHeight : bounds.height;
            return [width_2, height_2];
        }
        // If the image has intrinsic proportions, it's stretched to the specified dimension.
        // The unspecified dimension is computed using the specified dimension and the intrinsic proportions.
        if (hasIntrinsicProportion) {
            var width_3 = 0;
            var height_3 = 0;
            if (isLengthPercentage(first)) {
                width_3 = getAbsoluteValue(first, bounds.width);
            }
            else if (isLengthPercentage(second)) {
                height_3 = getAbsoluteValue(second, bounds.height);
            }
            if (isAuto(first)) {
                width_3 = height_3 * intrinsicProportion;
            }
            else if (!second || isAuto(second)) {
                height_3 = width_3 / intrinsicProportion;
            }
            return [width_3, height_3];
        }
        // If the image has no intrinsic proportions, it's stretched to the specified dimension.
        // The unspecified dimension is computed using the image's corresponding intrinsic dimension,
        // if there is one. If there is no such intrinsic dimension,
        // it becomes the corresponding dimension of the background positioning area.
        var width = null;
        var height = null;
        if (isLengthPercentage(first)) {
            width = getAbsoluteValue(first, bounds.width);
        }
        else if (second && isLengthPercentage(second)) {
            height = getAbsoluteValue(second, bounds.height);
        }
        if (width !== null && (!second || isAuto(second))) {
            height =
                hasIntrinsicWidth && hasIntrinsicHeight
                    ? (width / intrinsicWidth) * intrinsicHeight
                    : bounds.height;
        }
        if (height !== null && isAuto(first)) {
            width =
                hasIntrinsicWidth && hasIntrinsicHeight
                    ? (height / intrinsicHeight) * intrinsicWidth
                    : bounds.width;
        }
        if (width !== null && height !== null) {
            return [width, height];
        }
        throw new Error("Unable to calculate background-size for element");
    };
    var getBackgroundValueForIndex = function (values, index) {
        var value = values[index];
        if (typeof value === 'undefined') {
            return values[0];
        }
        return value;
    };
    var calculateBackgroundRepeatPath = function (repeat, _a, _b, backgroundPositioningArea, backgroundPaintingArea) {
        var x = _a[0], y = _a[1];
        var width = _b[0], height = _b[1];
        switch (repeat) {
            case 2 /* REPEAT_X */:
                return [
                    new Vector(Math.round(backgroundPositioningArea.left), Math.round(backgroundPositioningArea.top + y)),
                    new Vector(Math.round(backgroundPositioningArea.left + backgroundPositioningArea.width), Math.round(backgroundPositioningArea.top + y)),
                    new Vector(Math.round(backgroundPositioningArea.left + backgroundPositioningArea.width), Math.round(height + backgroundPositioningArea.top + y)),
                    new Vector(Math.round(backgroundPositioningArea.left), Math.round(height + backgroundPositioningArea.top + y))
                ];
            case 3 /* REPEAT_Y */:
                return [
                    new Vector(Math.round(backgroundPositioningArea.left + x), Math.round(backgroundPositioningArea.top)),
                    new Vector(Math.round(backgroundPositioningArea.left + x + width), Math.round(backgroundPositioningArea.top)),
                    new Vector(Math.round(backgroundPositioningArea.left + x + width), Math.round(backgroundPositioningArea.height + backgroundPositioningArea.top)),
                    new Vector(Math.round(backgroundPositioningArea.left + x), Math.round(backgroundPositioningArea.height + backgroundPositioningArea.top))
                ];
            case 1 /* NO_REPEAT */:
                return [
                    new Vector(Math.round(backgroundPositioningArea.left + x), Math.round(backgroundPositioningArea.top + y)),
                    new Vector(Math.round(backgroundPositioningArea.left + x + width), Math.round(backgroundPositioningArea.top + y)),
                    new Vector(Math.round(backgroundPositioningArea.left + x + width), Math.round(backgroundPositioningArea.top + y + height)),
                    new Vector(Math.round(backgroundPositioningArea.left + x), Math.round(backgroundPositioningArea.top + y + height))
                ];
            default:
                return [
                    new Vector(Math.round(backgroundPaintingArea.left), Math.round(backgroundPaintingArea.top)),
                    new Vector(Math.round(backgroundPaintingArea.left + backgroundPaintingArea.width), Math.round(backgroundPaintingArea.top)),
                    new Vector(Math.round(backgroundPaintingArea.left + backgroundPaintingArea.width), Math.round(backgroundPaintingArea.height + backgroundPaintingArea.top)),
                    new Vector(Math.round(backgroundPaintingArea.left), Math.round(backgroundPaintingArea.height + backgroundPaintingArea.top))
                ];
        }
    };

    var SMALL_IMAGE = 'data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7';

    var SAMPLE_TEXT = 'Hidden Text';
    var FontMetrics = /** @class */ (function () {
        function FontMetrics(document) {
            this._data = {};
            this._document = document;
        }
        FontMetrics.prototype.parseMetrics = function (fontFamily, fontSize) {
            var container = this._document.createElement('div');
            var img = this._document.createElement('img');
            var span = this._document.createElement('span');
            var body = this._document.body;
            container.style.visibility = 'hidden';
            container.style.fontFamily = fontFamily;
            container.style.fontSize = fontSize;
            container.style.margin = '0';
            container.style.padding = '0';
            container.style.whiteSpace = 'nowrap';
            body.appendChild(container);
            img.src = SMALL_IMAGE;
            img.width = 1;
            img.height = 1;
            img.style.margin = '0';
            img.style.padding = '0';
            img.style.verticalAlign = 'baseline';
            span.style.fontFamily = fontFamily;
            span.style.fontSize = fontSize;
            span.style.margin = '0';
            span.style.padding = '0';
            span.appendChild(this._document.createTextNode(SAMPLE_TEXT));
            container.appendChild(span);
            container.appendChild(img);
            var baseline = img.offsetTop - span.offsetTop + 2;
            container.removeChild(span);
            container.appendChild(this._document.createTextNode(SAMPLE_TEXT));
            container.style.lineHeight = 'normal';
            img.style.verticalAlign = 'super';
            var middle = img.offsetTop - container.offsetTop + 2;
            body.removeChild(container);
            return { baseline: baseline, middle: middle };
        };
        FontMetrics.prototype.getMetrics = function (fontFamily, fontSize) {
            var key = fontFamily + " " + fontSize;
            if (typeof this._data[key] === 'undefined') {
                this._data[key] = this.parseMetrics(fontFamily, fontSize);
            }
            return this._data[key];
        };
        return FontMetrics;
    }());

    var Renderer = /** @class */ (function () {
        function Renderer(context, options) {
            this.context = context;
            this.options = options;
        }
        return Renderer;
    }());

    var MASK_OFFSET = 10000;
    var CanvasRenderer = /** @class */ (function (_super) {
        __extends(CanvasRenderer, _super);
        function CanvasRenderer(context, options) {
            var _this = _super.call(this, context, options) || this;
            _this._activeEffects = [];
            _this.canvas = options.canvas ? options.canvas : document.createElement('canvas');
            _this.ctx = _this.canvas.getContext('2d');
            if (!options.canvas) {
                _this.canvas.width = Math.floor(options.width * options.scale);
                _this.canvas.height = Math.floor(options.height * options.scale);
                _this.canvas.style.width = options.width + "px";
                _this.canvas.style.height = options.height + "px";
            }
            _this.fontMetrics = new FontMetrics(document);
            _this.ctx.scale(_this.options.scale, _this.options.scale);
            _this.ctx.translate(-options.x, -options.y);
            _this.ctx.textBaseline = 'bottom';
            _this._activeEffects = [];
            _this.context.logger.debug("Canvas renderer initialized (" + options.width + "x" + options.height + ") with scale " + options.scale);
            return _this;
        }
        CanvasRenderer.prototype.applyEffects = function (effects) {
            var _this = this;
            while (this._activeEffects.length) {
                this.popEffect();
            }
            effects.forEach(function (effect) { return _this.applyEffect(effect); });
        };
        CanvasRenderer.prototype.applyEffect = function (effect) {
            this.ctx.save();
            if (isOpacityEffect(effect)) {
                this.ctx.globalAlpha = effect.opacity;
            }
            if (isTransformEffect(effect)) {
                this.ctx.translate(effect.offsetX, effect.offsetY);
                this.ctx.transform(effect.matrix[0], effect.matrix[1], effect.matrix[2], effect.matrix[3], effect.matrix[4], effect.matrix[5]);
                this.ctx.translate(-effect.offsetX, -effect.offsetY);
            }
            if (isClipEffect(effect)) {
                this.path(effect.path);
                this.ctx.clip();
            }
            this._activeEffects.push(effect);
        };
        CanvasRenderer.prototype.popEffect = function () {
            this._activeEffects.pop();
            this.ctx.restore();
        };
        CanvasRenderer.prototype.renderStack = function (stack) {
            return __awaiter(this, void 0, void 0, function () {
                var styles;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            styles = stack.element.container.styles;
                            if (!styles.isVisible()) return [3 /*break*/, 2];
                            return [4 /*yield*/, this.renderStackContent(stack)];
                        case 1:
                            _a.sent();
                            _a.label = 2;
                        case 2: return [2 /*return*/];
                    }
                });
            });
        };
        CanvasRenderer.prototype.renderNode = function (paint) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (contains(paint.container.flags, 16 /* DEBUG_RENDER */)) {
                                debugger;
                            }
                            if (!paint.container.styles.isVisible()) return [3 /*break*/, 3];
                            return [4 /*yield*/, this.renderNodeBackgroundAndBorders(paint)];
                        case 1:
                            _a.sent();
                            return [4 /*yield*/, this.renderNodeContent(paint)];
                        case 2:
                            _a.sent();
                            _a.label = 3;
                        case 3: return [2 /*return*/];
                    }
                });
            });
        };
        CanvasRenderer.prototype.renderTextWithLetterSpacing = function (text, letterSpacing, baseline) {
            var _this = this;
            if (letterSpacing === 0) {
                this.ctx.fillText(text.text, text.bounds.left, text.bounds.top + baseline);
            }
            else {
                var letters = segmentGraphemes(text.text);
                letters.reduce(function (left, letter) {
                    _this.ctx.fillText(letter, left, text.bounds.top + baseline);
                    return left + _this.ctx.measureText(letter).width;
                }, text.bounds.left);
            }
        };
        CanvasRenderer.prototype.createFontStyle = function (styles) {
            var fontVariant = styles.fontVariant
                .filter(function (variant) { return variant === 'normal' || variant === 'small-caps'; })
                .join('');
            var fontFamily = fixIOSSystemFonts(styles.fontFamily).join(', ');
            var fontSize = isDimensionToken(styles.fontSize)
                ? "" + styles.fontSize.number + styles.fontSize.unit
                : styles.fontSize.number + "px";
            return [
                [styles.fontStyle, fontVariant, styles.fontWeight, fontSize, fontFamily].join(' '),
                fontFamily,
                fontSize
            ];
        };
        CanvasRenderer.prototype.renderTextNode = function (text, styles) {
            return __awaiter(this, void 0, void 0, function () {
                var _a, font, fontFamily, fontSize, _b, baseline, middle, paintOrder;
                var _this = this;
                return __generator(this, function (_c) {
                    _a = this.createFontStyle(styles), font = _a[0], fontFamily = _a[1], fontSize = _a[2];
                    this.ctx.font = font;
                    this.ctx.direction = styles.direction === 1 /* RTL */ ? 'rtl' : 'ltr';
                    this.ctx.textAlign = 'left';
                    this.ctx.textBaseline = 'alphabetic';
                    _b = this.fontMetrics.getMetrics(fontFamily, fontSize), baseline = _b.baseline, middle = _b.middle;
                    paintOrder = styles.paintOrder;
                    text.textBounds.forEach(function (text) {
                        paintOrder.forEach(function (paintOrderLayer) {
                            switch (paintOrderLayer) {
                                case 0 /* FILL */:
                                    _this.ctx.fillStyle = asString(styles.color);
                                    _this.renderTextWithLetterSpacing(text, styles.letterSpacing, baseline);
                                    var textShadows = styles.textShadow;
                                    if (textShadows.length && text.text.trim().length) {
                                        textShadows
                                            .slice(0)
                                            .reverse()
                                            .forEach(function (textShadow) {
                                            _this.ctx.shadowColor = asString(textShadow.color);
                                            _this.ctx.shadowOffsetX = textShadow.offsetX.number * _this.options.scale;
                                            _this.ctx.shadowOffsetY = textShadow.offsetY.number * _this.options.scale;
                                            _this.ctx.shadowBlur = textShadow.blur.number;
                                            _this.renderTextWithLetterSpacing(text, styles.letterSpacing, baseline);
                                        });
                                        _this.ctx.shadowColor = '';
                                        _this.ctx.shadowOffsetX = 0;
                                        _this.ctx.shadowOffsetY = 0;
                                        _this.ctx.shadowBlur = 0;
                                    }
                                    if (styles.textDecorationLine.length) {
                                        _this.ctx.fillStyle = asString(styles.textDecorationColor || styles.color);
                                        styles.textDecorationLine.forEach(function (textDecorationLine) {
                                            switch (textDecorationLine) {
                                                case 1 /* UNDERLINE */:
                                                    // Draws a line at the baseline of the font
                                                    // TODO As some browsers display the line as more than 1px if the font-size is big,
                                                    // need to take that into account both in position and size
                                                    _this.ctx.fillRect(text.bounds.left, Math.round(text.bounds.top + baseline), text.bounds.width, 1);
                                                    break;
                                                case 2 /* OVERLINE */:
                                                    _this.ctx.fillRect(text.bounds.left, Math.round(text.bounds.top), text.bounds.width, 1);
                                                    break;
                                                case 3 /* LINE_THROUGH */:
                                                    // TODO try and find exact position for line-through
                                                    _this.ctx.fillRect(text.bounds.left, Math.ceil(text.bounds.top + middle), text.bounds.width, 1);
                                                    break;
                                            }
                                        });
                                    }
                                    break;
                                case 1 /* STROKE */:
                                    if (styles.webkitTextStrokeWidth && text.text.trim().length) {
                                        _this.ctx.strokeStyle = asString(styles.webkitTextStrokeColor);
                                        _this.ctx.lineWidth = styles.webkitTextStrokeWidth;
                                        // eslint-disable-next-line @typescript-eslint/no-explicit-any
                                        _this.ctx.lineJoin = !!window.chrome ? 'miter' : 'round';
                                        _this.ctx.strokeText(text.text, text.bounds.left, text.bounds.top + baseline);
                                    }
                                    _this.ctx.strokeStyle = '';
                                    _this.ctx.lineWidth = 0;
                                    _this.ctx.lineJoin = 'miter';
                                    break;
                            }
                        });
                    });
                    return [2 /*return*/];
                });
            });
        };
        CanvasRenderer.prototype.renderReplacedElement = function (container, curves, image) {
            if (image && container.intrinsicWidth > 0 && container.intrinsicHeight > 0) {
                var box = contentBox(container);
                var path = calculatePaddingBoxPath(curves);
                this.path(path);
                this.ctx.save();
                this.ctx.clip();
                this.ctx.drawImage(image, 0, 0, container.intrinsicWidth, container.intrinsicHeight, box.left, box.top, box.width, box.height);
                this.ctx.restore();
            }
        };
        CanvasRenderer.prototype.renderNodeContent = function (paint) {
            return __awaiter(this, void 0, void 0, function () {
                var container, curves, styles, _i, _a, child, image, image, iframeRenderer, canvas, size, _b, fontFamily, fontSize, baseline, bounds, x, textBounds, img, image, url, fontFamily, bounds;
                return __generator(this, function (_c) {
                    switch (_c.label) {
                        case 0:
                            this.applyEffects(paint.getEffects(4 /* CONTENT */));
                            container = paint.container;
                            curves = paint.curves;
                            styles = container.styles;
                            _i = 0, _a = container.textNodes;
                            _c.label = 1;
                        case 1:
                            if (!(_i < _a.length)) return [3 /*break*/, 4];
                            child = _a[_i];
                            return [4 /*yield*/, this.renderTextNode(child, styles)];
                        case 2:
                            _c.sent();
                            _c.label = 3;
                        case 3:
                            _i++;
                            return [3 /*break*/, 1];
                        case 4:
                            if (!(container instanceof ImageElementContainer)) return [3 /*break*/, 8];
                            _c.label = 5;
                        case 5:
                            _c.trys.push([5, 7, , 8]);
                            return [4 /*yield*/, this.context.cache.match(container.src)];
                        case 6:
                            image = _c.sent();
                            this.renderReplacedElement(container, curves, image);
                            return [3 /*break*/, 8];
                        case 7:
                            _c.sent();
                            this.context.logger.error("Error loading image " + container.src);
                            return [3 /*break*/, 8];
                        case 8:
                            if (container instanceof CanvasElementContainer) {
                                this.renderReplacedElement(container, curves, container.canvas);
                            }
                            if (!(container instanceof SVGElementContainer)) return [3 /*break*/, 12];
                            _c.label = 9;
                        case 9:
                            _c.trys.push([9, 11, , 12]);
                            return [4 /*yield*/, this.context.cache.match(container.svg)];
                        case 10:
                            image = _c.sent();
                            this.renderReplacedElement(container, curves, image);
                            return [3 /*break*/, 12];
                        case 11:
                            _c.sent();
                            this.context.logger.error("Error loading svg " + container.svg.substring(0, 255));
                            return [3 /*break*/, 12];
                        case 12:
                            if (!(container instanceof IFrameElementContainer && container.tree)) return [3 /*break*/, 14];
                            iframeRenderer = new CanvasRenderer(this.context, {
                                scale: this.options.scale,
                                backgroundColor: container.backgroundColor,
                                x: 0,
                                y: 0,
                                width: container.width,
                                height: container.height
                            });
                            return [4 /*yield*/, iframeRenderer.render(container.tree)];
                        case 13:
                            canvas = _c.sent();
                            if (container.width && container.height) {
                                this.ctx.drawImage(canvas, 0, 0, container.width, container.height, container.bounds.left, container.bounds.top, container.bounds.width, container.bounds.height);
                            }
                            _c.label = 14;
                        case 14:
                            if (container instanceof InputElementContainer) {
                                size = Math.min(container.bounds.width, container.bounds.height);
                                if (container.type === CHECKBOX) {
                                    if (container.checked) {
                                        this.ctx.save();
                                        this.path([
                                            new Vector(container.bounds.left + size * 0.39363, container.bounds.top + size * 0.79),
                                            new Vector(container.bounds.left + size * 0.16, container.bounds.top + size * 0.5549),
                                            new Vector(container.bounds.left + size * 0.27347, container.bounds.top + size * 0.44071),
                                            new Vector(container.bounds.left + size * 0.39694, container.bounds.top + size * 0.5649),
                                            new Vector(container.bounds.left + size * 0.72983, container.bounds.top + size * 0.23),
                                            new Vector(container.bounds.left + size * 0.84, container.bounds.top + size * 0.34085),
                                            new Vector(container.bounds.left + size * 0.39363, container.bounds.top + size * 0.79)
                                        ]);
                                        this.ctx.fillStyle = asString(INPUT_COLOR);
                                        this.ctx.fill();
                                        this.ctx.restore();
                                    }
                                }
                                else if (container.type === RADIO) {
                                    if (container.checked) {
                                        this.ctx.save();
                                        this.ctx.beginPath();
                                        this.ctx.arc(container.bounds.left + size / 2, container.bounds.top + size / 2, size / 4, 0, Math.PI * 2, true);
                                        this.ctx.fillStyle = asString(INPUT_COLOR);
                                        this.ctx.fill();
                                        this.ctx.restore();
                                    }
                                }
                            }
                            if (isTextInputElement(container) && container.value.length) {
                                _b = this.createFontStyle(styles), fontFamily = _b[0], fontSize = _b[1];
                                baseline = this.fontMetrics.getMetrics(fontFamily, fontSize).baseline;
                                this.ctx.font = fontFamily;
                                this.ctx.fillStyle = asString(styles.color);
                                this.ctx.textBaseline = 'alphabetic';
                                this.ctx.textAlign = canvasTextAlign(container.styles.textAlign);
                                bounds = contentBox(container);
                                x = 0;
                                switch (container.styles.textAlign) {
                                    case 1 /* CENTER */:
                                        x += bounds.width / 2;
                                        break;
                                    case 2 /* RIGHT */:
                                        x += bounds.width;
                                        break;
                                }
                                textBounds = bounds.add(x, 0, 0, -bounds.height / 2 + 1);
                                this.ctx.save();
                                this.path([
                                    new Vector(bounds.left, bounds.top),
                                    new Vector(bounds.left + bounds.width, bounds.top),
                                    new Vector(bounds.left + bounds.width, bounds.top + bounds.height),
                                    new Vector(bounds.left, bounds.top + bounds.height)
                                ]);
                                this.ctx.clip();
                                this.renderTextWithLetterSpacing(new TextBounds(container.value, textBounds), styles.letterSpacing, baseline);
                                this.ctx.restore();
                                this.ctx.textBaseline = 'alphabetic';
                                this.ctx.textAlign = 'left';
                            }
                            if (!contains(container.styles.display, 2048 /* LIST_ITEM */)) return [3 /*break*/, 20];
                            if (!(container.styles.listStyleImage !== null)) return [3 /*break*/, 19];
                            img = container.styles.listStyleImage;
                            if (!(img.type === 0 /* URL */)) return [3 /*break*/, 18];
                            image = void 0;
                            url = img.url;
                            _c.label = 15;
                        case 15:
                            _c.trys.push([15, 17, , 18]);
                            return [4 /*yield*/, this.context.cache.match(url)];
                        case 16:
                            image = _c.sent();
                            this.ctx.drawImage(image, container.bounds.left - (image.width + 10), container.bounds.top);
                            return [3 /*break*/, 18];
                        case 17:
                            _c.sent();
                            this.context.logger.error("Error loading list-style-image " + url);
                            return [3 /*break*/, 18];
                        case 18: return [3 /*break*/, 20];
                        case 19:
                            if (paint.listValue && container.styles.listStyleType !== -1 /* NONE */) {
                                fontFamily = this.createFontStyle(styles)[0];
                                this.ctx.font = fontFamily;
                                this.ctx.fillStyle = asString(styles.color);
                                this.ctx.textBaseline = 'middle';
                                this.ctx.textAlign = 'right';
                                bounds = new Bounds(container.bounds.left, container.bounds.top + getAbsoluteValue(container.styles.paddingTop, container.bounds.width), container.bounds.width, computeLineHeight(styles.lineHeight, styles.fontSize.number) / 2 + 1);
                                this.renderTextWithLetterSpacing(new TextBounds(paint.listValue, bounds), styles.letterSpacing, computeLineHeight(styles.lineHeight, styles.fontSize.number) / 2 + 2);
                                this.ctx.textBaseline = 'bottom';
                                this.ctx.textAlign = 'left';
                            }
                            _c.label = 20;
                        case 20: return [2 /*return*/];
                    }
                });
            });
        };
        CanvasRenderer.prototype.renderStackContent = function (stack) {
            return __awaiter(this, void 0, void 0, function () {
                var _i, _a, child, _b, _c, child, _d, _e, child, _f, _g, child, _h, _j, child, _k, _l, child, _m, _o, child;
                return __generator(this, function (_p) {
                    switch (_p.label) {
                        case 0:
                            if (contains(stack.element.container.flags, 16 /* DEBUG_RENDER */)) {
                                debugger;
                            }
                            // https://www.w3.org/TR/css-position-3/#painting-order
                            // 1. the background and borders of the element forming the stacking context.
                            return [4 /*yield*/, this.renderNodeBackgroundAndBorders(stack.element)];
                        case 1:
                            // https://www.w3.org/TR/css-position-3/#painting-order
                            // 1. the background and borders of the element forming the stacking context.
                            _p.sent();
                            _i = 0, _a = stack.negativeZIndex;
                            _p.label = 2;
                        case 2:
                            if (!(_i < _a.length)) return [3 /*break*/, 5];
                            child = _a[_i];
                            return [4 /*yield*/, this.renderStack(child)];
                        case 3:
                            _p.sent();
                            _p.label = 4;
                        case 4:
                            _i++;
                            return [3 /*break*/, 2];
                        case 5: 
                        // 3. For all its in-flow, non-positioned, block-level descendants in tree order:
                        return [4 /*yield*/, this.renderNodeContent(stack.element)];
                        case 6:
                            // 3. For all its in-flow, non-positioned, block-level descendants in tree order:
                            _p.sent();
                            _b = 0, _c = stack.nonInlineLevel;
                            _p.label = 7;
                        case 7:
                            if (!(_b < _c.length)) return [3 /*break*/, 10];
                            child = _c[_b];
                            return [4 /*yield*/, this.renderNode(child)];
                        case 8:
                            _p.sent();
                            _p.label = 9;
                        case 9:
                            _b++;
                            return [3 /*break*/, 7];
                        case 10:
                            _d = 0, _e = stack.nonPositionedFloats;
                            _p.label = 11;
                        case 11:
                            if (!(_d < _e.length)) return [3 /*break*/, 14];
                            child = _e[_d];
                            return [4 /*yield*/, this.renderStack(child)];
                        case 12:
                            _p.sent();
                            _p.label = 13;
                        case 13:
                            _d++;
                            return [3 /*break*/, 11];
                        case 14:
                            _f = 0, _g = stack.nonPositionedInlineLevel;
                            _p.label = 15;
                        case 15:
                            if (!(_f < _g.length)) return [3 /*break*/, 18];
                            child = _g[_f];
                            return [4 /*yield*/, this.renderStack(child)];
                        case 16:
                            _p.sent();
                            _p.label = 17;
                        case 17:
                            _f++;
                            return [3 /*break*/, 15];
                        case 18:
                            _h = 0, _j = stack.inlineLevel;
                            _p.label = 19;
                        case 19:
                            if (!(_h < _j.length)) return [3 /*break*/, 22];
                            child = _j[_h];
                            return [4 /*yield*/, this.renderNode(child)];
                        case 20:
                            _p.sent();
                            _p.label = 21;
                        case 21:
                            _h++;
                            return [3 /*break*/, 19];
                        case 22:
                            _k = 0, _l = stack.zeroOrAutoZIndexOrTransformedOrOpacity;
                            _p.label = 23;
                        case 23:
                            if (!(_k < _l.length)) return [3 /*break*/, 26];
                            child = _l[_k];
                            return [4 /*yield*/, this.renderStack(child)];
                        case 24:
                            _p.sent();
                            _p.label = 25;
                        case 25:
                            _k++;
                            return [3 /*break*/, 23];
                        case 26:
                            _m = 0, _o = stack.positiveZIndex;
                            _p.label = 27;
                        case 27:
                            if (!(_m < _o.length)) return [3 /*break*/, 30];
                            child = _o[_m];
                            return [4 /*yield*/, this.renderStack(child)];
                        case 28:
                            _p.sent();
                            _p.label = 29;
                        case 29:
                            _m++;
                            return [3 /*break*/, 27];
                        case 30: return [2 /*return*/];
                    }
                });
            });
        };
        CanvasRenderer.prototype.mask = function (paths) {
            this.ctx.beginPath();
            this.ctx.moveTo(0, 0);
            this.ctx.lineTo(this.canvas.width, 0);
            this.ctx.lineTo(this.canvas.width, this.canvas.height);
            this.ctx.lineTo(0, this.canvas.height);
            this.ctx.lineTo(0, 0);
            this.formatPath(paths.slice(0).reverse());
            this.ctx.closePath();
        };
        CanvasRenderer.prototype.path = function (paths) {
            this.ctx.beginPath();
            this.formatPath(paths);
            this.ctx.closePath();
        };
        CanvasRenderer.prototype.formatPath = function (paths) {
            var _this = this;
            paths.forEach(function (point, index) {
                var start = isBezierCurve(point) ? point.start : point;
                if (index === 0) {
                    _this.ctx.moveTo(start.x, start.y);
                }
                else {
                    _this.ctx.lineTo(start.x, start.y);
                }
                if (isBezierCurve(point)) {
                    _this.ctx.bezierCurveTo(point.startControl.x, point.startControl.y, point.endControl.x, point.endControl.y, point.end.x, point.end.y);
                }
            });
        };
        CanvasRenderer.prototype.renderRepeat = function (path, pattern, offsetX, offsetY) {
            this.path(path);
            this.ctx.fillStyle = pattern;
            this.ctx.translate(offsetX, offsetY);
            this.ctx.fill();
            this.ctx.translate(-offsetX, -offsetY);
        };
        CanvasRenderer.prototype.resizeImage = function (image, width, height) {
            var _a;
            if (image.width === width && image.height === height) {
                return image;
            }
            var ownerDocument = (_a = this.canvas.ownerDocument) !== null && _a !== void 0 ? _a : document;
            var canvas = ownerDocument.createElement('canvas');
            canvas.width = Math.max(1, width);
            canvas.height = Math.max(1, height);
            var ctx = canvas.getContext('2d');
            ctx.drawImage(image, 0, 0, image.width, image.height, 0, 0, width, height);
            return canvas;
        };
        CanvasRenderer.prototype.renderBackgroundImage = function (container) {
            return __awaiter(this, void 0, void 0, function () {
                var index, _loop_1, this_1, _i, _a, backgroundImage;
                return __generator(this, function (_b) {
                    switch (_b.label) {
                        case 0:
                            index = container.styles.backgroundImage.length - 1;
                            _loop_1 = function (backgroundImage) {
                                var image, url, _c, path, x, y, width, height, pattern, _d, path, x, y, width, height, _e, lineLength, x0, x1, y0, y1, canvas, ctx, gradient_1, pattern, _f, path, left, top_1, width, height, position, x, y, _g, rx, ry, radialGradient_1, midX, midY, f, invF;
                                return __generator(this, function (_h) {
                                    switch (_h.label) {
                                        case 0:
                                            if (!(backgroundImage.type === 0 /* URL */)) return [3 /*break*/, 5];
                                            image = void 0;
                                            url = backgroundImage.url;
                                            _h.label = 1;
                                        case 1:
                                            _h.trys.push([1, 3, , 4]);
                                            return [4 /*yield*/, this_1.context.cache.match(url)];
                                        case 2:
                                            image = _h.sent();
                                            return [3 /*break*/, 4];
                                        case 3:
                                            _h.sent();
                                            this_1.context.logger.error("Error loading background-image " + url);
                                            return [3 /*break*/, 4];
                                        case 4:
                                            if (image) {
                                                _c = calculateBackgroundRendering(container, index, [
                                                    image.width,
                                                    image.height,
                                                    image.width / image.height
                                                ]), path = _c[0], x = _c[1], y = _c[2], width = _c[3], height = _c[4];
                                                pattern = this_1.ctx.createPattern(this_1.resizeImage(image, width, height), 'repeat');
                                                this_1.renderRepeat(path, pattern, x, y);
                                            }
                                            return [3 /*break*/, 6];
                                        case 5:
                                            if (isLinearGradient(backgroundImage)) {
                                                _d = calculateBackgroundRendering(container, index, [null, null, null]), path = _d[0], x = _d[1], y = _d[2], width = _d[3], height = _d[4];
                                                _e = calculateGradientDirection(backgroundImage.angle, width, height), lineLength = _e[0], x0 = _e[1], x1 = _e[2], y0 = _e[3], y1 = _e[4];
                                                canvas = document.createElement('canvas');
                                                canvas.width = width;
                                                canvas.height = height;
                                                ctx = canvas.getContext('2d');
                                                gradient_1 = ctx.createLinearGradient(x0, y0, x1, y1);
                                                processColorStops(backgroundImage.stops, lineLength).forEach(function (colorStop) {
                                                    return gradient_1.addColorStop(colorStop.stop, asString(colorStop.color));
                                                });
                                                ctx.fillStyle = gradient_1;
                                                ctx.fillRect(0, 0, width, height);
                                                if (width > 0 && height > 0) {
                                                    pattern = this_1.ctx.createPattern(canvas, 'repeat');
                                                    this_1.renderRepeat(path, pattern, x, y);
                                                }
                                            }
                                            else if (isRadialGradient(backgroundImage)) {
                                                _f = calculateBackgroundRendering(container, index, [
                                                    null,
                                                    null,
                                                    null
                                                ]), path = _f[0], left = _f[1], top_1 = _f[2], width = _f[3], height = _f[4];
                                                position = backgroundImage.position.length === 0 ? [FIFTY_PERCENT] : backgroundImage.position;
                                                x = getAbsoluteValue(position[0], width);
                                                y = getAbsoluteValue(position[position.length - 1], height);
                                                _g = calculateRadius(backgroundImage, x, y, width, height), rx = _g[0], ry = _g[1];
                                                if (rx > 0 && ry > 0) {
                                                    radialGradient_1 = this_1.ctx.createRadialGradient(left + x, top_1 + y, 0, left + x, top_1 + y, rx);
                                                    processColorStops(backgroundImage.stops, rx * 2).forEach(function (colorStop) {
                                                        return radialGradient_1.addColorStop(colorStop.stop, asString(colorStop.color));
                                                    });
                                                    this_1.path(path);
                                                    this_1.ctx.fillStyle = radialGradient_1;
                                                    if (rx !== ry) {
                                                        midX = container.bounds.left + 0.5 * container.bounds.width;
                                                        midY = container.bounds.top + 0.5 * container.bounds.height;
                                                        f = ry / rx;
                                                        invF = 1 / f;
                                                        this_1.ctx.save();
                                                        this_1.ctx.translate(midX, midY);
                                                        this_1.ctx.transform(1, 0, 0, f, 0, 0);
                                                        this_1.ctx.translate(-midX, -midY);
                                                        this_1.ctx.fillRect(left, invF * (top_1 - midY) + midY, width, height * invF);
                                                        this_1.ctx.restore();
                                                    }
                                                    else {
                                                        this_1.ctx.fill();
                                                    }
                                                }
                                            }
                                            _h.label = 6;
                                        case 6:
                                            index--;
                                            return [2 /*return*/];
                                    }
                                });
                            };
                            this_1 = this;
                            _i = 0, _a = container.styles.backgroundImage.slice(0).reverse();
                            _b.label = 1;
                        case 1:
                            if (!(_i < _a.length)) return [3 /*break*/, 4];
                            backgroundImage = _a[_i];
                            return [5 /*yield**/, _loop_1(backgroundImage)];
                        case 2:
                            _b.sent();
                            _b.label = 3;
                        case 3:
                            _i++;
                            return [3 /*break*/, 1];
                        case 4: return [2 /*return*/];
                    }
                });
            });
        };
        CanvasRenderer.prototype.renderSolidBorder = function (color, side, curvePoints) {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    this.path(parsePathForBorder(curvePoints, side));
                    this.ctx.fillStyle = asString(color);
                    this.ctx.fill();
                    return [2 /*return*/];
                });
            });
        };
        CanvasRenderer.prototype.renderDoubleBorder = function (color, width, side, curvePoints) {
            return __awaiter(this, void 0, void 0, function () {
                var outerPaths, innerPaths;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (!(width < 3)) return [3 /*break*/, 2];
                            return [4 /*yield*/, this.renderSolidBorder(color, side, curvePoints)];
                        case 1:
                            _a.sent();
                            return [2 /*return*/];
                        case 2:
                            outerPaths = parsePathForBorderDoubleOuter(curvePoints, side);
                            this.path(outerPaths);
                            this.ctx.fillStyle = asString(color);
                            this.ctx.fill();
                            innerPaths = parsePathForBorderDoubleInner(curvePoints, side);
                            this.path(innerPaths);
                            this.ctx.fill();
                            return [2 /*return*/];
                    }
                });
            });
        };
        CanvasRenderer.prototype.renderNodeBackgroundAndBorders = function (paint) {
            return __awaiter(this, void 0, void 0, function () {
                var styles, hasBackground, borders, backgroundPaintingArea, side, _i, borders_1, border;
                var _this = this;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            this.applyEffects(paint.getEffects(2 /* BACKGROUND_BORDERS */));
                            styles = paint.container.styles;
                            hasBackground = !isTransparent(styles.backgroundColor) || styles.backgroundImage.length;
                            borders = [
                                { style: styles.borderTopStyle, color: styles.borderTopColor, width: styles.borderTopWidth },
                                { style: styles.borderRightStyle, color: styles.borderRightColor, width: styles.borderRightWidth },
                                { style: styles.borderBottomStyle, color: styles.borderBottomColor, width: styles.borderBottomWidth },
                                { style: styles.borderLeftStyle, color: styles.borderLeftColor, width: styles.borderLeftWidth }
                            ];
                            backgroundPaintingArea = calculateBackgroundCurvedPaintingArea(getBackgroundValueForIndex(styles.backgroundClip, 0), paint.curves);
                            if (!(hasBackground || styles.boxShadow.length)) return [3 /*break*/, 2];
                            this.ctx.save();
                            this.path(backgroundPaintingArea);
                            this.ctx.clip();
                            if (!isTransparent(styles.backgroundColor)) {
                                this.ctx.fillStyle = asString(styles.backgroundColor);
                                this.ctx.fill();
                            }
                            return [4 /*yield*/, this.renderBackgroundImage(paint.container)];
                        case 1:
                            _a.sent();
                            this.ctx.restore();
                            styles.boxShadow
                                .slice(0)
                                .reverse()
                                .forEach(function (shadow) {
                                _this.ctx.save();
                                var borderBoxArea = calculateBorderBoxPath(paint.curves);
                                var maskOffset = shadow.inset ? 0 : MASK_OFFSET;
                                var shadowPaintingArea = transformPath(borderBoxArea, -maskOffset + (shadow.inset ? 1 : -1) * shadow.spread.number, (shadow.inset ? 1 : -1) * shadow.spread.number, shadow.spread.number * (shadow.inset ? -2 : 2), shadow.spread.number * (shadow.inset ? -2 : 2));
                                if (shadow.inset) {
                                    _this.path(borderBoxArea);
                                    _this.ctx.clip();
                                    _this.mask(shadowPaintingArea);
                                }
                                else {
                                    _this.mask(borderBoxArea);
                                    _this.ctx.clip();
                                    _this.path(shadowPaintingArea);
                                }
                                _this.ctx.shadowOffsetX = shadow.offsetX.number + maskOffset;
                                _this.ctx.shadowOffsetY = shadow.offsetY.number;
                                _this.ctx.shadowColor = asString(shadow.color);
                                _this.ctx.shadowBlur = shadow.blur.number;
                                _this.ctx.fillStyle = shadow.inset ? asString(shadow.color) : 'rgba(0,0,0,1)';
                                _this.ctx.fill();
                                _this.ctx.restore();
                            });
                            _a.label = 2;
                        case 2:
                            side = 0;
                            _i = 0, borders_1 = borders;
                            _a.label = 3;
                        case 3:
                            if (!(_i < borders_1.length)) return [3 /*break*/, 13];
                            border = borders_1[_i];
                            if (!(border.style !== 0 /* NONE */ && !isTransparent(border.color) && border.width > 0)) return [3 /*break*/, 11];
                            if (!(border.style === 2 /* DASHED */)) return [3 /*break*/, 5];
                            return [4 /*yield*/, this.renderDashedDottedBorder(border.color, border.width, side, paint.curves, 2 /* DASHED */)];
                        case 4:
                            _a.sent();
                            return [3 /*break*/, 11];
                        case 5:
                            if (!(border.style === 3 /* DOTTED */)) return [3 /*break*/, 7];
                            return [4 /*yield*/, this.renderDashedDottedBorder(border.color, border.width, side, paint.curves, 3 /* DOTTED */)];
                        case 6:
                            _a.sent();
                            return [3 /*break*/, 11];
                        case 7:
                            if (!(border.style === 4 /* DOUBLE */)) return [3 /*break*/, 9];
                            return [4 /*yield*/, this.renderDoubleBorder(border.color, border.width, side, paint.curves)];
                        case 8:
                            _a.sent();
                            return [3 /*break*/, 11];
                        case 9: return [4 /*yield*/, this.renderSolidBorder(border.color, side, paint.curves)];
                        case 10:
                            _a.sent();
                            _a.label = 11;
                        case 11:
                            side++;
                            _a.label = 12;
                        case 12:
                            _i++;
                            return [3 /*break*/, 3];
                        case 13: return [2 /*return*/];
                    }
                });
            });
        };
        CanvasRenderer.prototype.renderDashedDottedBorder = function (color, width, side, curvePoints, style) {
            return __awaiter(this, void 0, void 0, function () {
                var strokePaths, boxPaths, startX, startY, endX, endY, length, dashLength, spaceLength, useLineDash, multiplier, numberOfDashes, minSpace, maxSpace, path1, path2, path1, path2;
                return __generator(this, function (_a) {
                    this.ctx.save();
                    strokePaths = parsePathForBorderStroke(curvePoints, side);
                    boxPaths = parsePathForBorder(curvePoints, side);
                    if (style === 2 /* DASHED */) {
                        this.path(boxPaths);
                        this.ctx.clip();
                    }
                    if (isBezierCurve(boxPaths[0])) {
                        startX = boxPaths[0].start.x;
                        startY = boxPaths[0].start.y;
                    }
                    else {
                        startX = boxPaths[0].x;
                        startY = boxPaths[0].y;
                    }
                    if (isBezierCurve(boxPaths[1])) {
                        endX = boxPaths[1].end.x;
                        endY = boxPaths[1].end.y;
                    }
                    else {
                        endX = boxPaths[1].x;
                        endY = boxPaths[1].y;
                    }
                    if (side === 0 || side === 2) {
                        length = Math.abs(startX - endX);
                    }
                    else {
                        length = Math.abs(startY - endY);
                    }
                    this.ctx.beginPath();
                    if (style === 3 /* DOTTED */) {
                        this.formatPath(strokePaths);
                    }
                    else {
                        this.formatPath(boxPaths.slice(0, 2));
                    }
                    dashLength = width < 3 ? width * 3 : width * 2;
                    spaceLength = width < 3 ? width * 2 : width;
                    if (style === 3 /* DOTTED */) {
                        dashLength = width;
                        spaceLength = width;
                    }
                    useLineDash = true;
                    if (length <= dashLength * 2) {
                        useLineDash = false;
                    }
                    else if (length <= dashLength * 2 + spaceLength) {
                        multiplier = length / (2 * dashLength + spaceLength);
                        dashLength *= multiplier;
                        spaceLength *= multiplier;
                    }
                    else {
                        numberOfDashes = Math.floor((length + spaceLength) / (dashLength + spaceLength));
                        minSpace = (length - numberOfDashes * dashLength) / (numberOfDashes - 1);
                        maxSpace = (length - (numberOfDashes + 1) * dashLength) / numberOfDashes;
                        spaceLength =
                            maxSpace <= 0 || Math.abs(spaceLength - minSpace) < Math.abs(spaceLength - maxSpace)
                                ? minSpace
                                : maxSpace;
                    }
                    if (useLineDash) {
                        if (style === 3 /* DOTTED */) {
                            this.ctx.setLineDash([0, dashLength + spaceLength]);
                        }
                        else {
                            this.ctx.setLineDash([dashLength, spaceLength]);
                        }
                    }
                    if (style === 3 /* DOTTED */) {
                        this.ctx.lineCap = 'round';
                        this.ctx.lineWidth = width;
                    }
                    else {
                        this.ctx.lineWidth = width * 2 + 1.1;
                    }
                    this.ctx.strokeStyle = asString(color);
                    this.ctx.stroke();
                    this.ctx.setLineDash([]);
                    // dashed round edge gap
                    if (style === 2 /* DASHED */) {
                        if (isBezierCurve(boxPaths[0])) {
                            path1 = boxPaths[3];
                            path2 = boxPaths[0];
                            this.ctx.beginPath();
                            this.formatPath([new Vector(path1.end.x, path1.end.y), new Vector(path2.start.x, path2.start.y)]);
                            this.ctx.stroke();
                        }
                        if (isBezierCurve(boxPaths[1])) {
                            path1 = boxPaths[1];
                            path2 = boxPaths[2];
                            this.ctx.beginPath();
                            this.formatPath([new Vector(path1.end.x, path1.end.y), new Vector(path2.start.x, path2.start.y)]);
                            this.ctx.stroke();
                        }
                    }
                    this.ctx.restore();
                    return [2 /*return*/];
                });
            });
        };
        CanvasRenderer.prototype.render = function (element) {
            return __awaiter(this, void 0, void 0, function () {
                var stack;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            if (this.options.backgroundColor) {
                                this.ctx.fillStyle = asString(this.options.backgroundColor);
                                this.ctx.fillRect(this.options.x, this.options.y, this.options.width, this.options.height);
                            }
                            stack = parseStackingContexts(element);
                            return [4 /*yield*/, this.renderStack(stack)];
                        case 1:
                            _a.sent();
                            this.applyEffects([]);
                            return [2 /*return*/, this.canvas];
                    }
                });
            });
        };
        return CanvasRenderer;
    }(Renderer));
    var isTextInputElement = function (container) {
        if (container instanceof TextareaElementContainer) {
            return true;
        }
        else if (container instanceof SelectElementContainer) {
            return true;
        }
        else if (container instanceof InputElementContainer && container.type !== RADIO && container.type !== CHECKBOX) {
            return true;
        }
        return false;
    };
    var calculateBackgroundCurvedPaintingArea = function (clip, curves) {
        switch (clip) {
            case 0 /* BORDER_BOX */:
                return calculateBorderBoxPath(curves);
            case 2 /* CONTENT_BOX */:
                return calculateContentBoxPath(curves);
            case 1 /* PADDING_BOX */:
            default:
                return calculatePaddingBoxPath(curves);
        }
    };
    var canvasTextAlign = function (textAlign) {
        switch (textAlign) {
            case 1 /* CENTER */:
                return 'center';
            case 2 /* RIGHT */:
                return 'right';
            case 0 /* LEFT */:
            default:
                return 'left';
        }
    };
    // see https://github.com/niklasvh/html2canvas/pull/2645
    var iOSBrokenFonts = ['-apple-system', 'system-ui'];
    var fixIOSSystemFonts = function (fontFamilies) {
        return /iPhone OS 15_(0|1)/.test(window.navigator.userAgent)
            ? fontFamilies.filter(function (fontFamily) { return iOSBrokenFonts.indexOf(fontFamily) === -1; })
            : fontFamilies;
    };

    var ForeignObjectRenderer = /** @class */ (function (_super) {
        __extends(ForeignObjectRenderer, _super);
        function ForeignObjectRenderer(context, options) {
            var _this = _super.call(this, context, options) || this;
            _this.canvas = options.canvas ? options.canvas : document.createElement('canvas');
            _this.ctx = _this.canvas.getContext('2d');
            _this.options = options;
            _this.canvas.width = Math.floor(options.width * options.scale);
            _this.canvas.height = Math.floor(options.height * options.scale);
            _this.canvas.style.width = options.width + "px";
            _this.canvas.style.height = options.height + "px";
            _this.ctx.scale(_this.options.scale, _this.options.scale);
            _this.ctx.translate(-options.x, -options.y);
            _this.context.logger.debug("EXPERIMENTAL ForeignObject renderer initialized (" + options.width + "x" + options.height + " at " + options.x + "," + options.y + ") with scale " + options.scale);
            return _this;
        }
        ForeignObjectRenderer.prototype.render = function (element) {
            return __awaiter(this, void 0, void 0, function () {
                var svg, img;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            svg = createForeignObjectSVG(this.options.width * this.options.scale, this.options.height * this.options.scale, this.options.scale, this.options.scale, element);
                            return [4 /*yield*/, loadSerializedSVG(svg)];
                        case 1:
                            img = _a.sent();
                            if (this.options.backgroundColor) {
                                this.ctx.fillStyle = asString(this.options.backgroundColor);
                                this.ctx.fillRect(0, 0, this.options.width * this.options.scale, this.options.height * this.options.scale);
                            }
                            this.ctx.drawImage(img, -this.options.x * this.options.scale, -this.options.y * this.options.scale);
                            return [2 /*return*/, this.canvas];
                    }
                });
            });
        };
        return ForeignObjectRenderer;
    }(Renderer));
    var loadSerializedSVG = function (svg) {
        return new Promise(function (resolve, reject) {
            var img = new Image();
            img.onload = function () {
                resolve(img);
            };
            img.onerror = reject;
            img.src = "data:image/svg+xml;charset=utf-8," + encodeURIComponent(new XMLSerializer().serializeToString(svg));
        });
    };

    var Logger = /** @class */ (function () {
        function Logger(_a) {
            var id = _a.id, enabled = _a.enabled;
            this.id = id;
            this.enabled = enabled;
            this.start = Date.now();
        }
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        Logger.prototype.debug = function () {
            var args = [];
            for (var _i = 0; _i < arguments.length; _i++) {
                args[_i] = arguments[_i];
            }
            if (this.enabled) {
                // eslint-disable-next-line no-console
                if (typeof window !== 'undefined' && window.console && typeof console.debug === 'function') {
                    // eslint-disable-next-line no-console
                    console.debug.apply(console, __spreadArray([this.id, this.getTime() + "ms"], args));
                }
                else {
                    this.info.apply(this, args);
                }
            }
        };
        Logger.prototype.getTime = function () {
            return Date.now() - this.start;
        };
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        Logger.prototype.info = function () {
            var args = [];
            for (var _i = 0; _i < arguments.length; _i++) {
                args[_i] = arguments[_i];
            }
            if (this.enabled) {
                // eslint-disable-next-line no-console
                if (typeof window !== 'undefined' && window.console && typeof console.info === 'function') {
                    // eslint-disable-next-line no-console
                    console.info.apply(console, __spreadArray([this.id, this.getTime() + "ms"], args));
                }
            }
        };
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        Logger.prototype.warn = function () {
            var args = [];
            for (var _i = 0; _i < arguments.length; _i++) {
                args[_i] = arguments[_i];
            }
            if (this.enabled) {
                // eslint-disable-next-line no-console
                if (typeof window !== 'undefined' && window.console && typeof console.warn === 'function') {
                    // eslint-disable-next-line no-console
                    console.warn.apply(console, __spreadArray([this.id, this.getTime() + "ms"], args));
                }
                else {
                    this.info.apply(this, args);
                }
            }
        };
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        Logger.prototype.error = function () {
            var args = [];
            for (var _i = 0; _i < arguments.length; _i++) {
                args[_i] = arguments[_i];
            }
            if (this.enabled) {
                // eslint-disable-next-line no-console
                if (typeof window !== 'undefined' && window.console && typeof console.error === 'function') {
                    // eslint-disable-next-line no-console
                    console.error.apply(console, __spreadArray([this.id, this.getTime() + "ms"], args));
                }
                else {
                    this.info.apply(this, args);
                }
            }
        };
        Logger.instances = {};
        return Logger;
    }());

    var Context = /** @class */ (function () {
        function Context(options, windowBounds) {
            var _a;
            this.windowBounds = windowBounds;
            this.instanceName = "#" + Context.instanceCount++;
            this.logger = new Logger({ id: this.instanceName, enabled: options.logging });
            this.cache = (_a = options.cache) !== null && _a !== void 0 ? _a : new Cache(this, options);
        }
        Context.instanceCount = 1;
        return Context;
    }());

    var html2canvas = function (element, options) {
        if (options === void 0) { options = {}; }
        return renderElement(element, options);
    };
    if (typeof window !== 'undefined') {
        CacheStorage.setContext(window);
    }
    var renderElement = function (element, opts) { return __awaiter(void 0, void 0, void 0, function () {
        var ownerDocument, defaultView, resourceOptions, contextOptions, windowOptions, windowBounds, context, foreignObjectRendering, cloneOptions, documentCloner, clonedElement, container, _a, width, height, left, top, backgroundColor, renderOptions, canvas, renderer, root, renderer;
        var _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t;
        return __generator(this, function (_u) {
            switch (_u.label) {
                case 0:
                    if (!element || typeof element !== 'object') {
                        return [2 /*return*/, Promise.reject('Invalid element provided as first argument')];
                    }
                    ownerDocument = element.ownerDocument;
                    if (!ownerDocument) {
                        throw new Error("Element is not attached to a Document");
                    }
                    defaultView = ownerDocument.defaultView;
                    if (!defaultView) {
                        throw new Error("Document is not attached to a Window");
                    }
                    resourceOptions = {
                        allowTaint: (_b = opts.allowTaint) !== null && _b !== void 0 ? _b : false,
                        imageTimeout: (_c = opts.imageTimeout) !== null && _c !== void 0 ? _c : 15000,
                        proxy: opts.proxy,
                        useCORS: (_d = opts.useCORS) !== null && _d !== void 0 ? _d : false
                    };
                    contextOptions = __assign({ logging: (_e = opts.logging) !== null && _e !== void 0 ? _e : true, cache: opts.cache }, resourceOptions);
                    windowOptions = {
                        windowWidth: (_f = opts.windowWidth) !== null && _f !== void 0 ? _f : defaultView.innerWidth,
                        windowHeight: (_g = opts.windowHeight) !== null && _g !== void 0 ? _g : defaultView.innerHeight,
                        scrollX: (_h = opts.scrollX) !== null && _h !== void 0 ? _h : defaultView.pageXOffset,
                        scrollY: (_j = opts.scrollY) !== null && _j !== void 0 ? _j : defaultView.pageYOffset
                    };
                    windowBounds = new Bounds(windowOptions.scrollX, windowOptions.scrollY, windowOptions.windowWidth, windowOptions.windowHeight);
                    context = new Context(contextOptions, windowBounds);
                    foreignObjectRendering = (_k = opts.foreignObjectRendering) !== null && _k !== void 0 ? _k : false;
                    cloneOptions = {
                        allowTaint: (_l = opts.allowTaint) !== null && _l !== void 0 ? _l : false,
                        onclone: opts.onclone,
                        ignoreElements: opts.ignoreElements,
                        inlineImages: foreignObjectRendering,
                        copyStyles: foreignObjectRendering
                    };
                    context.logger.debug("Starting document clone with size " + windowBounds.width + "x" + windowBounds.height + " scrolled to " + -windowBounds.left + "," + -windowBounds.top);
                    documentCloner = new DocumentCloner(context, element, cloneOptions);
                    clonedElement = documentCloner.clonedReferenceElement;
                    if (!clonedElement) {
                        return [2 /*return*/, Promise.reject("Unable to find element in cloned iframe")];
                    }
                    return [4 /*yield*/, documentCloner.toIFrame(ownerDocument, windowBounds)];
                case 1:
                    container = _u.sent();
                    _a = isBodyElement(clonedElement) || isHTMLElement(clonedElement)
                        ? parseDocumentSize(clonedElement.ownerDocument)
                        : parseBounds(context, clonedElement), width = _a.width, height = _a.height, left = _a.left, top = _a.top;
                    backgroundColor = parseBackgroundColor(context, clonedElement, opts.backgroundColor);
                    renderOptions = {
                        canvas: opts.canvas,
                        backgroundColor: backgroundColor,
                        scale: (_o = (_m = opts.scale) !== null && _m !== void 0 ? _m : defaultView.devicePixelRatio) !== null && _o !== void 0 ? _o : 1,
                        x: ((_p = opts.x) !== null && _p !== void 0 ? _p : 0) + left,
                        y: ((_q = opts.y) !== null && _q !== void 0 ? _q : 0) + top,
                        width: (_r = opts.width) !== null && _r !== void 0 ? _r : Math.ceil(width),
                        height: (_s = opts.height) !== null && _s !== void 0 ? _s : Math.ceil(height)
                    };
                    if (!foreignObjectRendering) return [3 /*break*/, 3];
                    context.logger.debug("Document cloned, using foreign object rendering");
                    renderer = new ForeignObjectRenderer(context, renderOptions);
                    return [4 /*yield*/, renderer.render(clonedElement)];
                case 2:
                    canvas = _u.sent();
                    return [3 /*break*/, 5];
                case 3:
                    context.logger.debug("Document cloned, element located at " + left + "," + top + " with size " + width + "x" + height + " using computed rendering");
                    context.logger.debug("Starting DOM parsing");
                    root = parseTree(context, clonedElement);
                    if (backgroundColor === root.styles.backgroundColor) {
                        root.styles.backgroundColor = COLORS.TRANSPARENT;
                    }
                    context.logger.debug("Starting renderer for element at " + renderOptions.x + "," + renderOptions.y + " with size " + renderOptions.width + "x" + renderOptions.height);
                    renderer = new CanvasRenderer(context, renderOptions);
                    return [4 /*yield*/, renderer.render(root)];
                case 4:
                    canvas = _u.sent();
                    _u.label = 5;
                case 5:
                    if ((_t = opts.removeContainer) !== null && _t !== void 0 ? _t : true) {
                        if (!DocumentCloner.destroy(container)) {
                            context.logger.error("Cannot detach cloned iframe as it is not in the DOM anymore");
                        }
                    }
                    context.logger.debug("Finished rendering");
                    return [2 /*return*/, canvas];
            }
        });
    }); };
    var parseBackgroundColor = function (context, element, backgroundColorOverride) {
        var ownerDocument = element.ownerDocument;
        // http://www.w3.org/TR/css3-background/#special-backgrounds
        var documentBackgroundColor = ownerDocument.documentElement
            ? parseColor(context, getComputedStyle(ownerDocument.documentElement).backgroundColor)
            : COLORS.TRANSPARENT;
        var bodyBackgroundColor = ownerDocument.body
            ? parseColor(context, getComputedStyle(ownerDocument.body).backgroundColor)
            : COLORS.TRANSPARENT;
        var defaultBackgroundColor = typeof backgroundColorOverride === 'string'
            ? parseColor(context, backgroundColorOverride)
            : backgroundColorOverride === null
                ? COLORS.TRANSPARENT
                : 0xffffffff;
        return element === ownerDocument.documentElement
            ? isTransparent(documentBackgroundColor)
                ? isTransparent(bodyBackgroundColor)
                    ? defaultBackgroundColor
                    : bodyBackgroundColor
                : documentBackgroundColor
            : defaultBackgroundColor;
    };

    return html2canvas;

})));
//# sourceMappingURL=html2canvas.js.map


/***/ }),

/***/ "./TSX/Input/openHistorian_Adapters_CsvInputAdapter/CSVOutput.tsx":
/*!************************************************************************!*\
  !*** ./TSX/Input/openHistorian_Adapters_CsvInputAdapter/CSVOutput.tsx ***!
  \************************************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _gpa_gemstone_common_pages__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @gpa-gemstone/common-pages */ "webpack/sharing/consume/default/@gpa-gemstone/common-pages/@gpa-gemstone/common-pages");
/* harmony import */ var _gpa_gemstone_common_pages__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_common_pages__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var moment__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! moment */ "webpack/sharing/consume/default/moment/moment?be5a");
/* harmony import */ var moment__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(moment__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @gpa-gemstone/gpa-symbols */ "webpack/sharing/consume/default/@gpa-gemstone/gpa-symbols/@gpa-gemstone/gpa-symbols?6dfb");
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_3___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_3__);
/* harmony import */ var _gpa_gemstone_react_graph__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! @gpa-gemstone/react-graph */ "./node_modules/@gpa-gemstone/react-graph/lib/index.js");
/* harmony import */ var _Common_TSX_GrafanaQueryFunctions__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../../../Common/TSX/GrafanaQueryFunctions */ "../Common/TSX/GrafanaQueryFunctions.tsx");
/* harmony import */ var _gpa_gemstone_helper_functions__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! @gpa-gemstone/helper-functions */ "./node_modules/@gpa-gemstone/helper-functions/lib/index.js");
/* harmony import */ var lodash__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! lodash */ "webpack/sharing/consume/default/lodash/lodash?552c");
/* harmony import */ var lodash__WEBPACK_IMPORTED_MODULE_7___default = /*#__PURE__*/__webpack_require__.n(lodash__WEBPACK_IMPORTED_MODULE_7__);
//******************************************************************************************************
//  CSVOutput.tsx - Gbtc
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








const CSVOutputAdapter = (props) => {
    const GrafanaQueries = new _Common_TSX_GrafanaQueryFunctions__WEBPACK_IMPORTED_MODULE_5__.GrafanaQueryFunctions(`${props.HomePath}api/grafana`);
    const containerRef = react__WEBPACK_IMPORTED_MODULE_0__.useRef(null);
    const { width } = (0,_gpa_gemstone_helper_functions__WEBPACK_IMPORTED_MODULE_6__.useGetContainerPosition)(containerRef);
    const [outputMeasurements, setOutputMeasurements] = react__WEBPACK_IMPORTED_MODULE_0__.useState([]);
    const [timeFilter, setTimeFilter] = react__WEBPACK_IMPORTED_MODULE_0__.useState({ start: moment__WEBPACK_IMPORTED_MODULE_2___default()().subtract(props.Settings.DefaultDuration, 'seconds').format('MM/DD/YYYY HH:mm:ss.SSS'), duration: props.Settings.DefaultDuration, unit: 's' });
    const [lines, setLines] = react__WEBPACK_IMPORTED_MODULE_0__.useState([]);
    const [status, setStatus] = react__WEBPACK_IMPORTED_MODULE_0__.useState('uninitiated');
    react__WEBPACK_IMPORTED_MODULE_0__.useEffect(() => {
        const measurements = MeasurementStringToList(props.ConnectionParameters);
        if (!lodash__WEBPACK_IMPORTED_MODULE_7___default().isEqual(outputMeasurements, measurements))
            setOutputMeasurements(measurements);
    }, [props.ConnectionParameters]);
    react__WEBPACK_IMPORTED_MODULE_0__.useEffect(() => {
        const handles = outputMeasurements.map((outputMeasurement) => GrafanaQueries.Query({
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
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12 p-0', style: { height: 200 }, ref: containerRef }, status === 'loading' ? react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_3__.ReactIcons.SpiningIcon, { Size: '50%' }) :
                react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_graph__WEBPACK_IMPORTED_MODULE_4__.Plot, { defaultTdomain: [moment__WEBPACK_IMPORTED_MODULE_2___default()(timeFilter.start).valueOf(), moment__WEBPACK_IMPORTED_MODULE_2___default()(timeFilter.start).add(timeFilter.duration, timeFilter.unit).valueOf()], height: 200, width: width, Ylabel: 'Measurements', legend: 'right', yDomain: 'AutoValue', Tlabel: 'Duration', useMetricFactors: false }, lines.map((line, i) => { var _a, _b; return (react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_graph__WEBPACK_IMPORTED_MODULE_4__.Line, { key: i, data: (_b = (_a = line.Data) === null || _a === void 0 ? void 0 : _a[0]) === null || _b === void 0 ? void 0 : _b.datapoints.map(d => [d[1], d[0]]), color: `var(--gpa-${i})`, lineStyle: '-', legend: line.Label })); }))))));
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

/***/ "./TSX/Input/openHistorian_Adapters_CsvInputAdapter/CSVSettings.tsx":
/*!**************************************************************************!*\
  !*** ./TSX/Input/openHistorian_Adapters_CsvInputAdapter/CSVSettings.tsx ***!
  \**************************************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?e49e");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @gpa-gemstone/react-forms */ "webpack/sharing/consume/default/@gpa-gemstone/react-forms/@gpa-gemstone/react-forms?045b");
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var lodash__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! lodash */ "webpack/sharing/consume/default/lodash/lodash?552c");
/* harmony import */ var lodash__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(lodash__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var _Common_TSX_Adapters_MeasurementSelector__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../../../../Common/TSX/Adapters/MeasurementSelector */ "../Common/TSX/Adapters/MeasurementSelector.tsx");
/* harmony import */ var _Common_TSX_Adapters_ColumnSelector__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../../../../Common/TSX/Adapters/ColumnSelector */ "../Common/TSX/Adapters/ColumnSelector.tsx");
/* harmony import */ var _Common_TSX_Adapters_HelperFunctions__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../../../../Common/TSX/Adapters/HelperFunctions */ "../Common/TSX/Adapters/HelperFunctions.tsx");
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
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_Common_TSX_Adapters_MeasurementSelector__WEBPACK_IMPORTED_MODULE_3__["default"], { SelectedPointTags: settings.OutputMeasurements, SetSelectedPointTags: handleSetOutputMeasurements, HomePath: props.HomePath }))),
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement(_Common_TSX_Adapters_ColumnSelector__WEBPACK_IMPORTED_MODULE_4__["default"], { SelectedPointTags: settings.OutputMeasurements, Columns: settings.ColumnMappings, SetColumns: handleSetColumns }))))
        : null));
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (CSVInputAdapterSettingsUI);


/***/ }),

/***/ "./TSX/Input/openHistorian_Adapters_CsvInputAdapter/openHistorian_Adapters_CsvInputAdapter.tsx":
/*!*****************************************************************************************************!*\
  !*** ./TSX/Input/openHistorian_Adapters_CsvInputAdapter/openHistorian_Adapters_CsvInputAdapter.tsx ***!
  \*****************************************************************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var _CSVSettings__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./CSVSettings */ "./TSX/Input/openHistorian_Adapters_CsvInputAdapter/CSVSettings.tsx");
/* harmony import */ var _CSVOutput__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./CSVOutput */ "./TSX/Input/openHistorian_Adapters_CsvInputAdapter/CSVOutput.tsx");
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
    Output: _CSVOutput__WEBPACK_IMPORTED_MODULE_1__["default"]
};
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = (CSVInputAdapter);


/***/ }),

/***/ "../Common/TSX/Adapters/ColumnSelector.tsx":
/*!*************************************************!*\
  !*** ../Common/TSX/Adapters/ColumnSelector.tsx ***!
  \*************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?620b");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @gpa-gemstone/react-forms */ "webpack/sharing/consume/default/@gpa-gemstone/react-forms/@gpa-gemstone/react-forms?f52a");
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var _gpa_gemstone_react_interactive__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @gpa-gemstone/react-interactive */ "webpack/sharing/consume/default/@gpa-gemstone/react-interactive/@gpa-gemstone/react-interactive?36fb");
/* harmony import */ var _gpa_gemstone_react_interactive__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_react_interactive__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! @gpa-gemstone/gpa-symbols */ "webpack/sharing/consume/default/@gpa-gemstone/gpa-symbols/@gpa-gemstone/gpa-symbols?46f0");
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

/***/ "../Common/TSX/Adapters/HelperFunctions.tsx":
/*!**************************************************!*\
  !*** ../Common/TSX/Adapters/HelperFunctions.tsx ***!
  \**************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   convertParametersToSettings: () => (/* binding */ convertParametersToSettings),
/* harmony export */   convertSettingsToParameters: () => (/* binding */ convertSettingsToParameters),
/* harmony export */   getHelperText: () => (/* binding */ getHelperText),
/* harmony export */   handleHistorianSearch: () => (/* binding */ handleHistorianSearch),
/* harmony export */   handleTimeFormatSearch: () => (/* binding */ handleTimeFormatSearch),
/* harmony export */   isISettingsKey: () => (/* binding */ isISettingsKey),
/* harmony export */   parseBoolean: () => (/* binding */ parseBoolean),
/* harmony export */   parseNumber: () => (/* binding */ parseNumber),
/* harmony export */   parseOutputMeasurements: () => (/* binding */ parseOutputMeasurements),
/* harmony export */   sortConParams: () => (/* binding */ sortConParams)
/* harmony export */ });
/* harmony import */ var _global__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../global */ "../Common/TSX/global.ts");
/* harmony import */ var lodash__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! lodash */ "webpack/sharing/consume/default/lodash/lodash?4ba6");
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
const sortConParams = (connParams) => {
    return lodash__WEBPACK_IMPORTED_MODULE_1___default().sortBy(connParams, 'Name');
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
        else
            valueStr = (_b = (_a = settings[param.Name]) === null || _a === void 0 ? void 0 : _a.toString()) !== null && _b !== void 0 ? _b : param.Value;
        return Object.assign(Object.assign({}, param), { Value: valueStr });
    });
    return newParams;
}
;
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


/***/ }),

/***/ "../Common/TSX/Adapters/MeasurementSelector.tsx":
/*!******************************************************!*\
  !*** ../Common/TSX/Adapters/MeasurementSelector.tsx ***!
  \******************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?620b");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @gpa-gemstone/react-forms */ "webpack/sharing/consume/default/@gpa-gemstone/react-forms/@gpa-gemstone/react-forms?f52a");
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @gpa-gemstone/gpa-symbols */ "webpack/sharing/consume/default/@gpa-gemstone/gpa-symbols/@gpa-gemstone/gpa-symbols?46f0");
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var _PointTagFilter__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./PointTagFilter */ "../Common/TSX/Adapters/PointTagFilter.tsx");
/* harmony import */ var _GrafanaQueryFunctions__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../GrafanaQueryFunctions */ "../Common/TSX/GrafanaQueryFunctions.tsx");
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
    var _a, _b, _c;
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
                ((_a = props.IsOutput) !== null && _a !== void 0 ? _a : false) ? 'Output' : 'Input',
                " Measurement Selector"),
            react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12' },
                    react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'row' },
                        react__WEBPACK_IMPORTED_MODULE_0__.createElement("div", { className: 'col-12' },
                            react__WEBPACK_IMPORTED_MODULE_0__.createElement(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__.SearchableSelect, { Search: handlePointTagSearch, Record: (_c = (_b = props.SelectedPointTags) === null || _b === void 0 ? void 0 : _b[props.SelectedPointTags.length - 1]) !== null && _c !== void 0 ? _c : { OutputMeasurement: '', Index: 0 }, Field: 'OutputMeasurement', Setter: handleSetPointTag, Style: { width: '100%' }, BtnStyle: { display: 'flex', paddingLeft: 0, alignItems: 'center' }, AllowCustom: false, Label: "Output Measurement", SearchLabel: '' }))),
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

/***/ "../Common/TSX/Adapters/PointTagFilter.tsx":
/*!*************************************************!*\
  !*** ../Common/TSX/Adapters/PointTagFilter.tsx ***!
  \*************************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! react */ "webpack/sharing/consume/default/react/react?620b");
/* harmony import */ var react__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(react__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! @gpa-gemstone/react-forms */ "webpack/sharing/consume/default/@gpa-gemstone/react-forms/@gpa-gemstone/react-forms?f52a");
/* harmony import */ var _gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(_gpa_gemstone_react_forms__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var _gpa_gemstone_gpa_symbols__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! @gpa-gemstone/gpa-symbols */ "webpack/sharing/consume/default/@gpa-gemstone/gpa-symbols/@gpa-gemstone/gpa-symbols?46f0");
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

/***/ "../Common/TSX/GrafanaQueryFunctions.tsx":
/*!***********************************************!*\
  !*** ../Common/TSX/GrafanaQueryFunctions.tsx ***!
  \***********************************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   GrafanaQueryFunctions: () => (/* binding */ GrafanaQueryFunctions)
/* harmony export */ });
/* harmony import */ var jquery__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! jquery */ "../Common/node_modules/jquery/dist/jquery.js");
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

/***/ "../Common/TSX/global.ts":
/*!*******************************!*\
  !*** ../Common/TSX/global.ts ***!
  \*******************************/
/***/ ((__unused_webpack_module, __webpack_exports__, __webpack_require__) => {

"use strict";
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