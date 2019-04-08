(function (angular) {
	"use strict";

	angular.module("tstfeed").factory("nugetUserService", ["$resource", "$http", nugetUserServiceFactory]);

	function nugetUserServiceFactory($resource) {
		return new nugetUserService($resource);
	}

	function nugetUserService($resource) {
		const srvc = this;

		srvc.getUserByName = function (name, successHandler, errorHandler) {
			$resource("api/api-key-generator/find-by-name/:name")
				.get({ name: name }, successHandler, errorHandler);
		};

		srvc.createUser = function (name, successHandler, errorHandler) {
			$resource('api/api-key-generator/generate-new-for/:name').save({ name: name }, {}, successHandler, errorHandler);
		};

		return srvc;
	}

})(window.angular);