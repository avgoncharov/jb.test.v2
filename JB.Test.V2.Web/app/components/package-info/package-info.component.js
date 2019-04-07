(
	function (angular) {
		"use strict";
		angular.module("tstfeed").component(
			"packageInfo",
			{
				templateUrl: "app/components/package-info/package-info.template.html?v=" + angular.module('tstfeed').info().version,
				controller: ["packageService", packageInfoController],
				bindings: {
					modalInstance: "<",
					resolve: "<"
				}
			});

		function packageInfoController(packageService) {
			const ctrl = this;
			ctrl.selected = "";
			ctrl.versions = [{ Id: 1 , Version: "1111"}];
		
			ctrl.$onInit = function () {
				ctrl.Id = ctrl.resolve.Id;
				packageService.getPackage(
					ctrl.resolve.Id,
					ctrl.resolve.Version,
					ctrl.successVersionsHandler,
					ctrl.errorHandler);
			};

			ctrl.successVersionsHandler = function(result) {
				ctrl.data = result;				
			};

			ctrl.errorHandler = function (err) {
				console.log(err);
			};
			

			ctrl.close = function () {
				ctrl.modalInstance.close({ dialogResult: "cancel" });
			};

		}
	}
)(window.angular);