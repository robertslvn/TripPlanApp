//tripsController.js

(function () {

    "use strict";

    // Getting existing module
    angular.module("app-trips")
        .controller("tripsController", tripsController)

    function tripsController($http) {

        var vm = this;

        vm.name = "Robert";

        vm.trips = [];

        vm.newTrip = {};
        vm.currTrip = {};

        vm.errorMessage = "";
        vm.isBusy = true;



        $http.get("/api/trips")
            .then(function (response) {
                //success
                angular.copy(response.data, vm.trips);
            }, function (error) {
                //failure
                vm.errorMessage = "Failed to load data" + error;
            })
            .finally(function () {
                vm.isBusy = false;
            });


        vm.delTrip = function (n) {

            vm.isBusy = true;
            vm.errorMessage = "";

            $http({
                method: 'DELETE',
                url: "/api/trips",
                params: { value: n }
            })
                .then(function (response) {
                    //success
                    vm.trips = [];
                    $http.get("/api/trips")
                        .then(function (response) {
                            //success
                            angular.copy(response.data, vm.trips);
                        }, function (error) {
                            //failure
                            vm.errorMessage = "Failed to load data" + error;
                        })
                        .finally(function () {
                            vm.isBusy = false;
                        });
                }, function () {
                    //failure
                    vm.errorMessage = "failed to save new trip";
                })
                .finally(function () {
                    vm.isBusy = false;
                });
        };

        vm.addTrip = function () {
            vm.isBusy = true;
            vm.errorMessage = "";

            $http.post("/api/trips", vm.newTrip)
                .then(function (response) {
                    //success
                    vm.trips.push(response.data);
                    vm.newTrip = {};
                }, function () {
                    //failure
                    vm.errorMessage = "failed to save new trip";
                })
                .finally(function () {
                    vm.isBusy = false;
                });
        };

      
    }

})();