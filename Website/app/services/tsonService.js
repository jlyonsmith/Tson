angular.module('tsonApp.services', ['tsonApp.config']) 
	.factory('tsonService', function($http, tsonApiUrl) {
		return {
			validate: function(text) {
				return $http.post(tsonApiUrl + "validate", { tson: text });
			},
			format: function(text) {
				return $http.post(tsonApiUrl + "format", { tson: text });
			},
			tson2json: function(text) {
				return $http.post(tsonApiUrl + "convert?toFormat=json", { tson: text });
			},
			tson2jsv: function(text) {
				return $http.post(tsonApiUrl + "convert?toFormat=jsv", { tson: text });
			},
			tson2xml: function(text) {
				return $http.post(tsonApiUrl + "convert?toFormat=xml", { tson: text });
			}
		};
	});