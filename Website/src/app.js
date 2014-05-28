var app = angular.module('tsonApp', ['ngRoute', 'tsonApp.services'])
	.config(function($routeProvider) {
		$routeProvider
			.when('/', {
				templateUrl: 'home/home.html',
				controller: 'homeController'
			})
			.when('/validate', {
				templateUrl: 'validate/validate.html',
				controller: 'validateController'
			})
			.when('/format', {
				templateUrl: 'format/format.html',
				controller: 'formatController'
			})
			.otherwise({
				redirectTo: '/'
			});
		});
