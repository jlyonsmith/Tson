app.controller('homeController', function($scope, $location, $analytics) {
	$analytics.pageTrack($location.path());
});