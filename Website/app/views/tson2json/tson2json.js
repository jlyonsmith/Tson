angular.module('tsonApp').controller('Tson2JsonController', function($scope, $location, $analytics, tsonService) {
	$analytics.pageTrack($location.path());
	$scope.result = null;

	$scope.submitForm = function() {
		tsonService.tson2json($scope.tson)
			.success(function(data, status, headers) {
				$scope.result = data;
			})
			.error(function(data) {
				$scope.result = data;
			});
		};

	$scope.tsonChanged = function() {
		$scope.result = null;
	};
});