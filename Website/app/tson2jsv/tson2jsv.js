app.controller('tson2JsvController', function($scope, tsonService) {
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
});