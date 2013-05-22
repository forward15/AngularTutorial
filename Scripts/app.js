var TodoApp = angular.module("TodoApp", ["ngResource"]).
    config(function ($routeProvider) {
        $routeProvider.
            when('/', { controller: ListCtrl, templateUrl: 'list.html' }).
            when('/new', { controller: CreateCtrl, templateUrl: 'details.html' }).
            when('/edit/:editId', { controller: EditCtrl, templateUrl: 'details.html' }).
            otherwise({ redirectTo: '/' });
        
    });

TodoApp.factory('Todo', function ($resource) {
    return $resource('/api/todo/:id', { id: '@id' }, { update: { method: 'PUT' } });
});

TodoApp.directive('sorted', function() {
    return {
        scope: true,
        transclude: true,
        template: '<a ng-click="do_sort()" ng-transclude></a>' +
            '<span ng-show="do_show(true)"><i class="icon-circle-arrow-down"></i></span>' +
            '<span ng-show="do_show(false)"><i class="icon-circle-arrow-up"></i></span>',
        // the ng-transclude directive tells AngularJS where we want the contents of the element put 
        // (they are put immediately after the element containing this attribute).
        controller: function($scope, $element, $attrs) {
            $scope.sort = $attrs.sorted;

            $scope.do_sort = function() { $scope.sort_by($scope.sort); };

            $scope.do_show = function(asc) {
                return (asc != $scope.is_descending) && ($scope.sort_order == $scope.sort);
            };
        }
    };
});

var CreateCtrl = function ($scope, $location, Todo) {
    $scope.action = "Add";
    $scope.save = function () {
        Todo.save($scope.item, function() {
            $location.path('/');
        });
    };
};

var EditCtrl = function($scope, $location, $routeParams, Todo) {
    var id = $routeParams.editId;
    $scope.action = "Edit";
    $scope.item = Todo.get({ id: id });
    
    $scope.save = function () {
        Todo.update({id:id}, $scope.item, function() {
            $location.path('/');
        });
    };
};

var ListCtrl = function ($scope, $location, Todo) {
    $scope.search = function() {
        //$scope.items = Todo.query({sort: $scope.sort_order, desc: $scope.is_descending });
        Todo.query({
                q:$scope.query,
            sort: $scope.sort_order,
            desc: $scope.is_descending,
            offset: $scope.offset,
            limit: $scope.limit
        },
            function (data) {
                $scope.more = data.length === $scope.limit;
                $scope.items = $scope.items.concat(data);
        });
    };

    $scope.sort_by = function (col) {
        if ($scope.sort_order === col) {
            $scope.is_descending = !$scope.is_descending;
        } else {
            $scope.sort_order = col;
            $scope.is_descending = false;
        }
        $scope.reset();
    };

    $scope.show_more = function() {
        $scope.offset += $scope.limit;
        $scope.search();
    };

    $scope.has_more = function() {
        return $scope.more;
    };
    
    $scope.reset = function () {
        $scope.limit = 15;
        $scope.offset = 0;
        $scope.items = [];
        $scope.more = true;
        
        $scope.search();
    };
    
    $scope.delete = function () {
        var itemId = this.item.Id;
        Todo.delete({ id: itemId }, function () {
                    $("#item_" + itemId).fadeOut();
                });
    };
        
    $scope.add = function () {
        
    };
        
    $scope.do_show = function (asc, col) {
        return (asc != $scope.is_descending) && ($scope.sort_order == col);
    };
    
    $scope.sort_order = "Priority";
    $scope.is_descending = false;

    $scope.reset();

};