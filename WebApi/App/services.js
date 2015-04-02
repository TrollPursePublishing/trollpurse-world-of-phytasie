﻿'use strict';

// Demonstrate how to register services
// In this case it is a simple value service.

var getAsync = function (url, $q, $http) {
    var q = $q.defer();
    $http.get(url)
    .success(function (data) {
        q.resolve({ data: data });
    })
    .error(function (msg, code) {
        q.reject(msg);
    });
    return q.promise;
};

var getAsyncData = function (url, data, $q, $http) {
    var q = $q.defer();
    $http.get(url, data)
    .success(function (data) {
        q.resolve({ data: data });
    })
    .error(function (msg, code) {
        q.reject(msg);
    });
    return q.promise;
};

var postAsync = function (url, data, $q, $http) {
    var q = $q.defer();
    $http.post(url, data)
    .success(function (data) {
        q.resolve({ data: data });
    })
    .error(function (msg, code) {
        q.reject(msg);
    });
    return q.promise;
};

angular.module('app.services', [])


    .factory('CommandService', ['$http', '$q', function ($http, $q) {
        var command = {};
        command.submit = function (data, userId) {
            //console.log(data);
            var q = $q.defer();
            $http.get('api/Command/' + userId + '/' + data)
            .success(function (data) {
                q.resolve({ data: data });
            })
            .error(function (msg, code) {
                q.reject(msg);
            });
            return q.promise;
        };

        return command;
    }])

    .factory('MessageService', ['$http', '$q', '$rootScope', '$', function ($http, $q, $rootScope, $) {
        var message = {};
        message.ChatHub = {};
        message.chatConnected = false;
        var proxy = null;


        message.startupServices = function () {
            $(function () {
                $('[data-toggle="tooltip"]').tooltip({ container: 'body' });
                console.log('Enabling tooltips.');
            });
            var q = $q.defer();
            if (!message.chatConnected) {
                $(function () {
                    message.ChatHub = $.connection.chatHub;
                    message.ChatHub.client.broadcastMessage = function (playerName, msg) {
                        console.log('broadcast', playerName + ':' + msg);
                        $rootScope.$emit('chat', { name: playerName, msg: msg });
                    }

                    message.ChatHub.client.broadcastJoin = function (playerName) {
                        console.log('broadcast', playerName + ':' + 'join');
                        $rootScope.$emit('playerjoin', { name: playerName });
                    }
                    $.connection.hub.start()
                    .done(function () { message.chatConnected = true; console.log('signalR', 'Connected!'); q.resolve('success'); })
                    .fail(function () { message.chatConnected = false; console.log('signalR', 'Not Connected :('); q.reject('failure') });
                });
            }
            return q.promise;
        };

        message.send = function (playerName, msg, locationId) {
            message.ChatHub.server.send(playerName, msg, locationId);
        };

        message.joinLocation = function (locationId, playerName) {
            message.ChatHub.server.joinLocation(locationId, playerName);
        };

        message.leaveLocation = function (locationId) {
            message.ChatHub.server.leaveLocation(locationId);
        };

        return message;

    }])

    .factory('NotificationService', ['$http', '$q', function ($http, $q) {
        var notifications = {};

        notifications.getEvents = function () {
            return getAsync('api/message/events', $q, $http);
        };

        notifications.getPlayerAcheivements = function (userId) {
            return getAsync('api/ach/' + userId, $q, $http);
        };

        notifications.getAllPlayerAchievements = function () {
            return getAsync('api/ach/latest', $q, $http);
        };

        return notifications;
    }])

    .factory('RanksService', ['$http', '$q', function ($http, $q) {
        var ranks = {};

        //TODO: http get call
        ranks.get = function () {
            return getAsync('api/ranks/topten', $q, $http);
        };

        ranks.getById = function (userId) {
            return getAsync('api/ranks/' + userId, $q, $http);
        };

        ranks.getAll = function () {
            return getAsync('api/ranks/all', $q, $http);
        };

        return ranks;
    }])

    .factory('AccountService', ['$http', '$q', function ($http, $q) {
        var acc = {};

        acc.confirmEmail = function (hash, confirmationCode) {
            return getAsync('api/account/confirm/' + hash + '/' + confirmationCode, $q, $http);
        };

        acc.doNotDisturb = function (hash) {
            return getAsync('api/account/donotdisturb/' + hash, $q, $http);
        };

        acc.registerForUpdates = function (playerId) {
            return getAsync('api/account/sendmail/' + playerId, $q, $http);
        };

        acc.resetPassword = function (id, password, confirmPassword) {
            console.log('Hit endpoint');
            return getAsync('api/account/password/reset/' + id + '/' + password + '/' + confirmPassword, $q, $http);
        };

        acc.resetConfirm = function (hash, securityStamp) {
            return getAsync('api/account/password/' + hash + '/' + securityStamp, $q, $http);
        };

        return acc;
    }])

    .factory('UserService', ['$http', '$q', function ($http, $q) {
        var user = {};
        user.isLoggedIn = false;
        user.user;

        user.getUserData = function (userId) {
            return getAsync('api/player/' + userId, $q, $http);
        };

        user.getUserImage = function (userId) {
            return '/Images/userprofileimages/' + userId + '.svg';
        };

        user.login = function (enteredusername, password) {
            var data = { username: enteredusername, password: password };
            return postAsync('api/account/login', data, $q, $http);
        };

        user.logout = function (playerId) {
            return getAsync('api/account/logout/' + playerId, $q, $http);
        };

        user.register = function (username, password, confirmpassword, email, gender) {
            var data = { username: username, password: password, confirmpassword: confirmpassword, email: email, gender: gender };
            return postAsync('api/account/register', data, $q, $http);
        };

        user.getPlayerData = function (playerId) {
            return getAsync('api/player/' + playerId, $q, $http);
        };

        user.reset = function (email) {
            var data = { email: email };
            return postAsync('api/account/password/reset', data, $q, $http);
        };

        return user;
    }])

    .value('version', '0.0.5.7')
    .value('gamename', 'AdventureQuestGame');