app.controller('tson2JsonController', function($scope, tsonService) {
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
});