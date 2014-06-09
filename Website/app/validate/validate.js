app.controller('validateController', function($scope, $location, $analytics, tsonService) {
	$analytics.pageTrack($location.path());
	$scope.result = null;

	$scope.submitForm = function() {
			tsonService.validate($scope.tson)
				.success(function(data, status, headers) {
					$scope.result = data;
				});
		};
	});