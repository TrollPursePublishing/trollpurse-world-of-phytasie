﻿'use strict';

angular.module('app.directives', [])

    .directive('gameview', [function () {
        return {
            scope: true,
            restrict: 'EA',
            replace: true,
            template: '<div id="gamebody" style="overflow:auto; max-height: 600px; min-height: 600px;">' +
                          '<p ng-repeat="message in messages track by $index">{{message}}</p>' +
                      '</div>',
            link: function (scope, element, attrs, controller) {
                scope.$watch('messages', function (messages) {
                    element[0].scrollTop = element[0].scrollHeight;
                }, true);
            }
        }
    }])

    .directive('appVersion', ['version', function (version) {
        return function (scope, elm, attrs) {
            elm.text(version);
        };
    }]);