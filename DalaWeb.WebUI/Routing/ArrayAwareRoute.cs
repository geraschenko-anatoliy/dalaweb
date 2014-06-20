using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace DalaWeb.WebUI.Routing
{
public class ArrayAwareRoute : Route
{
  public ArrayAwareRoute(string url, IRouteHandler routeHandler)
    : base(url, routeHandler)
  {
  }

  public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary routeValues)
  {
    NameValueCollection enumerables = new NameValueCollection();

    foreach (KeyValuePair<string, object> routeValue in routeValues)
    {
      IEnumerable<string> values = routeValue.Value as IEnumerable<string>;

      // collects all enumerable route values
      if (values != null)
      {
        foreach (string value in values)
        {
          enumerables.Add(routeValue.Key, value);
        }
      }
    }

    // removes all enumerable route values so they are not processed by the base class
    foreach (string key in enumerables.AllKeys)
    {
      routeValues.Remove(key);
    }

    // lets the base class generate a URL
    VirtualPathData path = base.GetVirtualPath(requestContext, routeValues);

    Uri requestUrl = requestContext.HttpContext.Request.Url;
    if (enumerables.Count > 0 && requestUrl != null && path != null)
    {
      string authority = requestUrl.GetLeftPart(UriPartial.Authority);
      Uri authorityUri = new Uri(authority);
      Uri url = new Uri(authorityUri, path.VirtualPath);
      UriBuilder builder = new UriBuilder(url);

      NameValueCollection queryString = HttpUtility.ParseQueryString(builder.Query);

      // extends the URL's query string with the provided enumerable route values
      queryString.Add(enumerables);

      builder.Query = queryString.ToString();

      path.VirtualPath = builder.Uri.PathAndQuery.TrimStart('/');
    }

    return path;
  }
}
}