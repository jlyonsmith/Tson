app.controller('json2TsonController', function($scope, $location, $analytics, tsonService) {
	$analytics.pageTrack($location.path());
	$scope.result = null;

	$scope.submitForm = function() {
		tsonService.json2tson($scope.json)
			.success(function(data, status, headers) {
				$scope.result = data;
			})
			.error(function(data) {
				$scope.result = data;
			});
		};

	$scope.jsonChanged = function() {
		$scope.result = null;
	};
});