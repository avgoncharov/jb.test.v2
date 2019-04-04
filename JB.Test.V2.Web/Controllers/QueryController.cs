using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace JB.Test.V2.Web.Controllers
{
	[RoutePrefix("query")]
	public class QueryController : ApiController
	{
	}

	/*
	 * {
			"@id": "http://localhost:65370/v3/registration3-gz/",
			"@type": "RegistrationsBaseUrl/3.4.0",
			"comment": "Base URL of Azure storage where NuGet package registration info is stored in GZIP format. This base URL does not include SemVer 2.0.0 packages."
		},
		{
			"@id": "http://localhost:65370/v3/registration3-gz-semver2/",
			"@type": "RegistrationsBaseUrl/3.6.0",
			"comment": "Base URL of Azure storage where NuGet package registration info is stored in GZIP format. This base URL includes SemVer 2.0.0 packages."
		},
	 *
	 */
}
