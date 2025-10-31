<h1>WM.ProductsApi</h1>
<p>
  A clean, <strong>SOLID</strong> ASP.NET Core Web API implementing the Western Mutual Jr. Developer assessment:
  CRUD Products with in-memory storage, two fixed buyers, and a deactivation notification.
</p>

<hr />

<h2>Requirements</h2>
<ul>
  <li>.NET 8 SDK</li>
  <li>Visual Studio 2022 (or <code>dotnet</code> CLI)</li>
</ul>

<hr />

<h2>Getting Started</h2>

<h3>Run (Visual Studio)</h3>
<ol>
  <li>Open the solution in <strong>Visual Studio 2022</strong>.</li>
  <li>Set <strong>WM.ProductsApi</strong> as the startup project.</li>
  <li>Press <strong>F5</strong> (or <strong>Ctrl+F5</strong>).</li>
  <li>Swagger will open at: <code>https://localhost:&lt;port&gt;/swagger</code></li>
</ol>

<h3>Run (CLI)</h3>
<pre><code class="language-bash"># from repo root
cd WM.ProductsApi
dotnet run
</code></pre>
<p>
  Swagger UI: <code>http://localhost:5000/swagger</code> (or the port shown in console)
</p>

<hr />

<h2>API Overview</h2>

<h3>Buyers (fixed list)</h3>
<p>Copy-paste these IDs when creating/updating products:</p>
<ul>
  <li><strong>Johnny Buyer</strong> — <code>49ec2a8703224eea9dec16b22546477e</code></li>
  <li><strong>Jennie Purchaser</strong> — <code>a790a7b6bf2a48569066c46306c3332d</code></li>
</ul>

<h3>Endpoints</h3>

<h4>Create product</h4>
<pre><code class="language-http">POST /api/products
Content-Type: application/json
</code></pre>
<pre><code class="language-json">{
  "sku": "ABC-001",
  "title": "Blue Widget",
  "description": "Example",
  "buyerId": "49ec2a8703224eea9dec16b22546477e",
  "active": true
}
</code></pre>
<ul>
  <li><strong>201 Created</strong> (Location header <code>/api/products/ABC-001</code>)</li>
  <li><strong>400 Bad Request</strong> (invalid buyer)</li>
  <li><strong>409 Conflict</strong> (duplicate SKU)</li>
</ul>

<h4>Get one</h4>
<pre><code class="language-http">GET /api/products/ABC-001
</code></pre>
<ul>
  <li><strong>200 OK</strong> / <strong>404 Not Found</strong></li>
</ul>

<h4>List (filters)</h4>
<pre><code class="language-http">GET /api/products?active=true&amp;buyerId=49ec2a8703224eea9dec16b22546477e&amp;search=blue
</code></pre>
<ul>
  <li><strong>200 OK</strong></li>
</ul>

<h4>Update</h4>
<pre><code class="language-http">PUT /api/products/ABC-001
Content-Type: application/json
</code></pre>
<pre><code class="language-json">{
  "title": "Blue Widget v2",
  "description": "Updated",
  "buyerId": "49ec2a8703224eea9dec16b22546477e",
  "active": false
}
</code></pre>
<ul>
  <li><strong>200 OK</strong> / <strong>400 Bad Request</strong> / <strong>404 Not Found</strong></li>
  <li><em>Side effect:</em> if <code>active</code> flips from <code>true</code> → <code>false</code>, a console message is printed (via notifier).</li>
</ul>

<h4>Delete</h4>
<pre><code class="language-http">DELETE /api/products/ABC-001
</code></pre>
<ul>
  <li><strong>204 No Content</strong> (idempotent)</li>
</ul>

<hr />

<h2>Configuration (JSON persistence)</h2>
<p>Products are stored in memory; optionally a JSON snapshot is kept on disk.</p>
<p><code>appsettings.json</code>:</p>
<pre><code class="language-json">{
  "Persistence": {
    "FilePath": "AppData/products.json"
  }
}
</code></pre>
<ul>
  <li>To <strong>change or disable</strong> persistence, set <code>FilePath</code> to a different location, or remove the decorator registration in <code>Program.cs</code>.</li>
  <li>On startup, if the file is missing/empty, the API starts with an empty store.</li>
</ul>

<hr />

<h2>Tests</h2>
<p>Run all tests (unit + integration):</p>
<pre><code class="language-bash"># from repo root
dotnet test
</code></pre>
<p>Integration tests spin up the API in-memory and verify HTTP status codes, filters, and the deactivation notifier.</p>

<hr />

<h2>Project Structure (high level)</h2>
<pre><code>WM.ProductsApi/
  Contracts/          # DTOs + mapping
  Controllers/        # REST controllers
  Domain/             # Abstractions, errors, query type
  Application/        # Business rules + services
  Infrastructure/     # InMemory repo, JSON persistence, adapters
  Data/               # SeedData for buyers
  Program.cs          # DI + Swagger

WM.ProductsApi.Tests/
  Application/        # Unit tests for rules/services
  Infrastructure/     # Unit tests for repo
  Integration/        # End-to-end tests (WebApplicationFactory)
</code></pre>

<hr />

<h2>Notes</h2>
<ul>
  <li>The API adheres to <strong>SOLID</strong> principles; dependencies are inverted via DI and all layers are testable.</li>
  <li>Deactivation prints a console message as required.</li>
  <li>Swagger includes XML comments for better docs (if building with XML docs enabled).</li>
</ul>
