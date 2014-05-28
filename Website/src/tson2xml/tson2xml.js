app.controller('tson2XmlController', function($scope, tsonService) {
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
});