app.controller('validateController', function($scope, tsonService) {
	$scope.result = null;

	$scope.submitForm = function() {
			tsonService.validate($scope.tson)
				.success(function(data, status, headers) {
					$scope.result = data;
				});
		};
	});