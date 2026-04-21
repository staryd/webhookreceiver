# webhookreceiver

This API accepts `GET` and `POST` requests on any path:

- `GET /{*id}`
- `POST /{*id}`

Use either of these authentication methods:

- Query string: `?code=YOUR_AUTH_CODE`
- Header: `Authorization: Bearer YOUR_AUTH_CODE`

Optional query parameters:

- `returnStatus=###` to force the HTTP status code returned by the API
- `code=...` if you are not using the `Authorization` header

## GET example

```bash
curl "http://localhost:5000/my/test/path?code=test"
```

## POST example

```bash
curl -X POST "http://localhost:5000/my/test/path?code=test" \
  -H "Content-Type: application/json" \
  -d '{"message":"hello"}'
```

## Bearer token example

```bash
curl -X POST "http://localhost:5000/my/test/path" \
  -H "Authorization: Bearer test" \
  -H "Content-Type: application/json" \
  -d '{"message":"hello"}'
```

Each request is written to a local file containing:

- the requested path and query string
- the request headers
- the request body for `POST` requests
