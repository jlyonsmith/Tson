angular.module('tsonApp.services') 
	.factory('tsonService', function($http, tsonApiUrl) {
		return {
			validate: function(text) {
				return $http.post(tsonApiUrl + "validate", { tson: text });
			},
			format: function(text) {
				return $http.post(tsonApiUrl + "format", { tson: text });
			},
			tson2json: function(text) {
				return $http.post(tsonApiUrl + "convert?toFormat=json", { data: text });
			},
			tson2jsv: function(text) {
				return $http.post(tsonApiUrl + "convert?toFormat=jsv", { data: text });
			},
			tson2xml: function(text) {
				return $http.post(tsonApiUrl + "convert?toFormat=xml", { data: text });
			},
			json2tson: function(text) {
				return $http.post(tsonApiUrl + "convert?toFormat=tson&fromFormat=json", { data: text });
			}
		};
	});