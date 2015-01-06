angular.module('tsonApp').controller('HomeController', function($scope, $location, $analytics) {
	$analytics.pageTrack($location.path());
});