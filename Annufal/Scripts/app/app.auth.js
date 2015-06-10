(function () {
	var app = angular.module('annufal');

	//--------------------------------------- Config -------------------------------------------

	//JWT token issuance URL
	app.constant('API_TOKEN_URL', 'api/token');

	//back-end authorization
	app.config(['$httpProvider', function ($httpProvider) {
		$httpProvider.interceptors.push('authInterceptor');
	}]);

	//routing authorization checks
	app.run(['$rootScope', '$location', 'authSvc', function ($rootScope, $location, authSvc) {
		$rootScope.$on('$routeChangeStart', function (event, args) {
			if (args.access !== undefined) { //if access conditions
				authorizationResult = authSvc.authorizeRoute(args.access);

				if (authorizationResult === 'LoginRequired')
					$location.path('/display-message/Login requis');
				else if (authorizationResult === 'NotAuthorized')
					$location.path('/display-message/Acces interdit').replace();
			}
		});
	}]);

	//--------------------------------------- Services ------------------------------------------

	//intercepts requests to API
	//if login request -> saved returned token
	//if other request -> attached saved token (if any)
	app.factory('authInterceptor', ['API_URL', 'API_TOKEN_URL', 'authSvc', function (API_URL, API_TOKEN_URL, authSvc) {
		return {
			request: function (config) {
				//if req to API and token is saved -> attach it
				var token = authSvc.getAuthToken();
				if (config.url.indexOf(API_URL) !== -1 && token) {
					config.headers.Authorization = 'Bearer ' + token;
				}

				return config;
			},
			response: function (res) {
				//if req to api/token and token is returned -> save it
				if (res.config.url.indexOf(API_TOKEN_URL) !== -1 && res.data.access_token) {
					authSvc.saveAuthToken(res.data.access_token);
				}

				return res;
			}
		}
	}]);

	//authentication service
	app.service('authSvc', ['$window', function ($window) {
		var self = this;

		self.parseJwtToken = function (token) {
			var base64Url = token.split('.')[1];
			var base64 = base64Url.replace('-', '+').replace('_', '/');
			return JSON.parse($window.atob(base64));
		};

		self.getAuthToken = function () {
			return $window.localStorage['authToken'];
		};

		self.saveAuthToken = function (token) {
			$window.localStorage['authToken'] = token;
			//save other common params
			if (token) {
				var params = self.parseJwtToken(token);
				if (params.unique_name)
					$window.localStorage['user_name'] = params.unique_name;
				if (params.role)
					$window.localStorage['user_roles'] = params.role;
			}
		};

		self.deleteAuthToken = function () {
			$window.localStorage.removeItem('authToken');
			$window.localStorage.removeItem('user_name');
			$window.localStorage.removeItem('user_roles');
		};

		self.isLoggedIn = function () {
			var token = self.getAuthToken();
			if (token) {
				var params = self.parseJwtToken(token);
				return Math.round(new Date().getTime() / 1000) <= params.exp; //Date().getTime() returns ms
			}
			else
				return false;
		};

		self.loggedInUserName = function () {
			return $window.localStorage['user_name'];
		};

		self.isAdmin = function () {
			return self.isInRole('Admin');
		};

		self.isInRole = function (role) {
			var found = false;
			var roles = $window.localStorage['user_roles']

			if (roles) {
				roles.split(',').forEach(function (r) {
					if (r.toUpperCase() === role.toUpperCase()) {
						found = true;
					}
				});
			}

			return found;
		};

		//authorize an angular route access
		self.authorizeRoute = function (access) {
			if (!access.requiresLogin)
				return 'Authorized';

			if (!self.isLoggedIn())
				return 'LoginRequired';

			//requiresLogin + loggedIn => now check roles requirement
			if (!access.requiredPermissions)
				return 'Authorized';

			switch (access.permissionType) {
				case 'AtLeastOne':
					var result = 'NotAuthorized';
					//if any of the required role is in user profile => Authorized
					access.requiredPermissions.forEach(function (rp) {
						if (self.isInRole(rp))
							result = 'Authorized';
					});
					return result;
				case 'All':
					var result = 'Authorized';
					//if any of the required role is not in user profile => NotAuthorized
					access.requiredPermissions.forEach(function (rp) {
						if (!self.isInRole(rp))
							result = 'Not Authorized';
					});
					return result;
			}

			//unknown case... in doubt => not authorized
			return 'NotAuthorized';
		};
	}]);
})();