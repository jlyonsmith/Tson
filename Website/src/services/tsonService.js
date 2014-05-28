angular.module('tsonApp.services',[]) 
	.factory('tsonService', function($http) {
		var tsonUrl = "http://localhost:1340"

		return {
			validate: function(text) {
				return $http.post(tsonUrl + "/validate", { tson: text });
			},
			format: function(text) {
				return $http.post(tsonUrl + "/format", { tson: text });
			},
			tson2json: function(text) {
				return $http.post(tsonUrl + "/convert?toFormat=json", { tson: text });
			},
			tson2jsv: function(text) {
				return $http.post(tsonUrl + "/convert?toFormat=jsv", { tson: text });
			},
			tson2xml: function(text) {
				return $http.post(tsonUrl + "/convert?toFormat=xml", { tson: text });
			}
		};
	});