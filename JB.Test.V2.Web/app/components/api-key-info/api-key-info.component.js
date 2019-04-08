(
	function (angular) {
		"use strict";
		angular.module("tstfeed").component(
			"apiKeyInfo",
			{
				templateUrl: "app/components/api-key-info/api-key-info.template.html?v=" + angular.module('tstfeed').info().version,
				controller: ["nugetUserService", packageInfoController],
				bindings: {
					modalInstance: "<",
					resolve: "<"
				}
			});

		function packageInfoController(nugetUserService) {
			const ctrl = this;
			ctrl.userName = '';
			
			ctrl.$onInit = function () {
				
			};

			ctrl.generate = function () {
				nugetUserService.getUserByName(
					ctrl.userName,
					ctrl.successFindUser,
					ctrl.failedFindUser);
			};

			ctrl.successFindUser = function (result) {
				ctrl.userName = result.Name;
				ctrl.apiKey = result.ApiKey;
			};

			ctrl.failedFindUser = function (error) {
				if (error.status === 404) {
					nugetUserService.createUser(
						ctrl.userName, 
						ctrl.successFindUser, 
						ctrl.errorHandler);
				}

				ctrl.errorHandler(error);
			};

			ctrl.successVersionsHandler = function (result) {
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