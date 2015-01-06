angular.module('tsonApp').controller('Tson2JsvController', function($scope, $location, $analytics, tsonService) {
	$analytics.pageTrack($location.path());
	$scope.result = null;

	$scope.submitForm = function() {
		tsonService.tson2jsv($scope.tson)
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