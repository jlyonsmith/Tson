angular.module('tsonApp').controller('FormatController', function($scope, $location, $analytics, tsonService) {
	$analytics.pageTrack($location.path());
	$scope.result = null;

	$scope.submitForm = function() {
			$scope.result = null;
			tsonService.format($scope.tson)
				.success(function(data) {
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