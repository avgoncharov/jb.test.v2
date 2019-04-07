(function (angular) {
	"use strict";

	angular.module("tstfeed").component(
		"commonInfo",
		{
			templateUrl: "app/components/common-info/common-info.template.html?v=" + angular.module('tstfeed').info().version,
			controller: ["packageService", "$uibModal" , commonInfoController]
		});

	function commonInfoController(packageService,$uibModal) {
		const ctrl = this;
		ctrl.data = [];

		ctrl.Id = "";
		ctrl.Version = "";
		ctrl.Description = "";
			 
		ctrl.$onInit = function () {
			ctrl.applyFilter();
		};

		ctrl.applyFilter = function () {
			packageService.findByFilter(
				ctrl.Id,				
				ctrl.Version,
				ctrl.Description,
				ctrl.successHandler,
				ctrl.errorHandler);
		};

		ctrl.successHandler = function (result) {
			ctrl.data = result.data ? result.data: result;
		};

		ctrl.errorHandler = function(error) {
			ctrl.data = [];
			alert("Can't get history: " + error.data.Message);
			console.log(error);
		};

		ctrl.open = function(id, version) {
			var modalInst = $uibModal.open(
				{
					animation: true,
					backdrop: false,
					component: "packageInfo",
					resolve: {
						Id: function () {
							return id;
						},
						Version: function(){ return version;}
					},
					size: "lg"
				});

			modalInst.result.then(function (rslt) {
			});
		};

	}
})(window.angular);