var app = angular.module('tsonApp', ['ngRoute'])
	.config(function($routeProvider) {
		$routeProvider
			.when('/', {
				templateUrl: 'home/home.html',
				controller: 'HomeController'
			})
			.when('/validate', {
				templateUrl: 'validate/validate.html',
				controller: 'ValidateController'
			})
			.when('/format', {
				templateUrl: 'format/format.html',
				controller: 'FormatController'
			})
			.otherwise({
				redirectTo: '/'
			});
		});
