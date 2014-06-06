var app = angular.module('tsonApp', ['ngRoute', 'tsonApp.services', 'tsonApp.version'])
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
			.when('/tson2jsv', {
				templateUrl: 'tson2jsv/tson2jsv.html',
				controller: 'tson2JsvController'
			})
			.when('/tson2json', {
				templateUrl: 'tson2json/tson2json.html',
				controller: 'tson2JsonController'
			})
			.when('/tson2xml', {
				templateUrl: 'tson2xml/tson2xml.html',
				controller: 'tson2XmlController'
			})
			.otherwise({
				redirectTo: '/'
			});
		});
