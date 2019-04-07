(function (angular) {
	"use strict";

	angular.module("tstfeed").factory("packageService", ["$resource", "$http", packageServiceFactory]);

	function packageServiceFactory($resource, $http) {
		return new PackageService($resource, $http);
	}

	function PackageService($resource, $http) {
		const srvc = this;
						
		srvc.getPackage = function(id, version, successHandler, errorHandler) {
			$resource("api/packages/:id/:version")
				.get({ id: id, version: version }, successHandler, errorHandler);
		};

		srvc.findByFilter = function (
			idPattern,			
			versionPattern,
			descriptionPattern,
			successHandler,
			errorHandler) {
			$http.post('api/packages', {
				Id: idPattern,				
				Version: versionPattern,
				Description: descriptionPattern
			},
				{}).then(successHandler, errorHandler);
		};

		return srvc;
	}

})(window.angular);