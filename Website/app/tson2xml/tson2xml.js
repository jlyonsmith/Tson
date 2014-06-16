app.controller('tson2XmlController', function($scope, $location, $analytics, tsonService) {
	$analytics.pageTrack($location.path());
	$scope.result = null;

	$scope.submitForm = function() {
		tsonService.tson2xml($scope.tson)
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