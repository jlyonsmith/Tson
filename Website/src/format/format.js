app.controller('formatController', function($scope, tsonService) {
	$scope.result = null;

	$scope.submitForm = function() {
			tsonService.format($scope.tson)
				.success(function(data, status, headers) {
					$scope.result = data;
				});
		};
});