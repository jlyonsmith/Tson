"use strict";

angular.module('tsonApp.services', ['tsonApp.config']);

angular.module('tsonApp', [
		'ngRoute', 
		'tsonApp.services', 
		'tsonApp.version', 
		'angulartics', 
		'angulartics.google.analytics'
		])
	.config(function($routeProvider, $analyticsProvider) {
		$analyticsProvider.virtualPageviews(false);
		$routeProvider
			.when('/', {
				templateUrl: 'views/home/home.html',
				controller: 'HomeController'
			})
			.when('/validate', {
				templateUrl: 'views/validate/validate.html',
				controller: 'ValidateController'
			})
			.when('/format', {
				templateUrl: 'views/format/format.html',
				controller: 'FormatController'
			})
			.when('/tson2jsv', {
				templateUrl: 'views/tson2jsv/tson2jsv.html',
				controller: 'Tson2JsvController'
			})
			.when('/tson2json', {
				templateUrl: 'views/tson2json/tson2json.html',
				controller: 'Tson2JsonController'
			})
			.when('/tson2xml', {
				templateUrl: 'views/tson2xml/tson2xml.html',
				controller: 'Tson2XmlController'
			})
			.when('/json2tson', {
				templateUrl: 'views/json2tson/json2tson.html',
				controller: 'Json2TsonController'
			})
			.otherwise({
				redirectTo: '/'
			});
		})
    .controller('TsonAppController', function($scope, tsonApiVersion) {
        $scope.version = tsonApiVersion;
    });
