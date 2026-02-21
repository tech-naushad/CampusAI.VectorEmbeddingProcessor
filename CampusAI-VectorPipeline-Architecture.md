# 🧠 CampusAI Vector Pipeline

> An end-to-end AI-powered semantic search pipeline built with **.NET 10**, **Hugging Face embeddings**, and **Pinecone vector database** — enabling intelligent, context-aware Q&A over structured campus data.

---

## 🚀 Project Overview

CampusAI Vector Pipeline is a production-ready data ingestion and semantic search system that transforms structured relational data into high-dimensional vector embeddings stored in a cloud vector database. The system enables downstream AI applications to perform **semantic similarity search** across campus information including locations, opening hours, directions, and transport options.

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        CampusAI Pipeline                        │
│                                                                 │
│  ┌──────────────┐     ┌─────────────────┐     ┌─────────────┐  │
│  │  MS SQL      │────▶│  Embedding      │────▶│  Pinecone   │  │
│  │  Server      │     │  Service        │     │  Vector DB  │  │
│  │  (Source)    │     │  (HuggingFace)  │     │  (Sink)     │  │
│  └──────────────┘     └─────────────────┘     └─────────────┘  │
│         │                      │                      │         │
│    Entity Data            float[384]             Namespaced     │
│    (Structured)           Vectors              Vector Records   │
└─────────────────────────────────────────────────────────────────┘
```

### Data Flow

```
MS SQL Server
    │
    │  Raw entities (Campus, Hours, Directions, Transport)
    ▼
IVectorizable Abstraction
    │
    │  GetVectorText() → plain text representation
    ▼
HuggingFace Inference API
    │  sentence-transformers/all-MiniLM-L6-v2
    │  Single batch HTTP call → float[384] per entity
    ▼
VectorRecord { Id, Values, Metadata }
    │
    │  Parallel upsert per namespace
    ▼
Pinecone Vector Database
    ├── namespace: campus
    ├── namespace: opening-hours
    ├── namespace: directions
    └── namespace: transport
```

---

## 🛠️ Tech Stack

| Layer | Technology | Purpose |
|---|---|---|
| **Language** | C# 12 / .NET 10 | Core application runtime |
| **Data Source** | Microsoft SQL Server | Structured campus data storage |
| **ORM / Data Access** | Entity Framework Core | Database querying and entity mapping |
| **Embedding Model** | `sentence-transformers/all-MiniLM-L6-v2` | Text → vector conversion (384 dimensions) |
| **Embedding API** | Hugging Face Inference API | Hosted model inference endpoint |
| **Vector Database** | Pinecone (Serverless) | Vector storage and similarity search |
| **HTTP Client** | `System.Net.Http.HttpClient` | REST API communication |
| **Serialization** | `System.Text.Json` | JSON payload construction |
| **Configuration** | `Microsoft.Extensions.Configuration` | Secrets and settings management |
| **DI Container** | `Microsoft.Extensions.DependencyInjection` | Service registration and lifetime management |
| **Parallelism** | TPL (`Task.WhenAll`) | Concurrent embedding and upsert operations |

---

## ✨ Key Engineering Highlights

### Batch Embedding (Single HTTP Call)
Instead of making one API call per entity, all texts are sent in a single batched request to Hugging Face, dramatically reducing latency and API overhead.

```csharp
// One HTTP call for all entities — not N calls
var embeddings = await embeddingProvider.GenerateEmbeddingsAsync(texts);
var records = entities.Zip(embeddings, (entity, embedding) => new VectorRecord
{
    Id       = entity.GetVectorId(),
    Values   = embedding,
    Metadata = entity.GetMetadata()
}).ToList();
```

### Parallel Pipeline Execution (TPL)
All four data domains are vectorized and upserted concurrently using `Task.WhenAll`, cutting total pipeline runtime by ~75%.

```csharp
// Vectorize all domains in parallel
var (campusVectors, hoursVectors, directionsVectors, transportVectors) =
    await Task.WhenAll(
        campuses.ToVectorRecordsAsync(_embeddingProvider),
        hours.ToVectorRecordsAsync(_embeddingProvider),
        directions.ToVectorRecordsAsync(_embeddingProvider),
        transport.ToVectorRecordsAsync(_embeddingProvider)
    );

// Upsert all namespaces in parallel
await Task.WhenAll(
    _pineconeService.UpsertAsync(campusVectors,     PineconeNamespaces.Campus),
    _pineconeService.UpsertAsync(hoursVectors,      PineconeNamespaces.Hours),
    _pineconeService.UpsertAsync(directionsVectors, PineconeNamespaces.Directions),
    _pineconeService.UpsertAsync(transportVectors,  PineconeNamespaces.Transport)
);
```

### Clean Abstraction via Interfaces
Every entity implements `IVectorizable`, decoupling business logic from vector infrastructure.

```csharp
public interface IVectorizable
{
    string GetVectorId();
    string GetVectorText();
    Dictionary<string, object> GetMetadata();
}
```

### Namespace-Scoped Vector Storage
Each entity type is stored in its own Pinecone namespace, enabling scoped semantic search and efficient targeted retrieval without cross-domain noise.

| Namespace | Entity | Use Case |
|---|---|---|
| `campus` | Campus locations | "Where is the engineering building?" |
| `opening-hours` | Business hours | "Is the library open on Sunday?" |
| `directions` | Navigation info | "How do I get to the student union?" |
| `transport` | Transport options | "What buses go to the main campus?" |

### Pinecone Batch Upsert (100 vectors per request)
Automatic chunking handles large datasets within Pinecone's per-request limits.

```csharp
var batches = records
    .Select((r, i) => new { r, i })
    .GroupBy(x => x.i / 100)
    .Select(g => g.Select(x => x.r).ToList());

foreach (var batch in batches)
    await UpsertBatchAsync(batch, ns);
```

---

## 📁 Project Structure

```
CampusAI.VectorPipeline/
├── Configuration/
│   └── PineconeOptions.cs          # Strongly-typed config binding
├── Embeddings/
│   ├── IEmbeddingProvider.cs       # Embedding abstraction
│   └── HuggingFaceEmbeddingProvider.cs  # HF Inference API client
├── Vector/
│   ├── IVectorizable.cs            # Entity contract
│   ├── VectorRecord.cs             # Pinecone vector model
│   └── Vectorizer.cs               # Batch vectorization extension
├── Services/
│   ├── IPineconeService.cs         # Pinecone abstraction
│   └── PineconeService.cs          # Pinecone REST API client
├── Constants/
│   └── PineconeNamespaces.cs       # Namespace constants
└── appsettings-example.json        # Configuration template
```

---

## ⚙️ Configuration

```json
{
  "Pinecone": {
    "ApiKey": "your-pinecone-api-key",
    "Host": "your-index-name.pinecone.io"
  },
  "Embedding": {
    "HuggingFace": {
      "ApiKey": "hf_your_api_key_here",
      "Model": "sentence-transformers/all-MiniLM-L6-v2"
    }
  }
}
```

---

## 📐 Embedding Model Details

| Property | Value |
|---|---|
| **Model** | `sentence-transformers/all-MiniLM-L6-v2` |
| **Dimensions** | 384 |
| **Max Tokens** | 256 |
| **Similarity Metric** | Cosine |
| **Provider** | Hugging Face Inference API |
| **Endpoint** | `router.huggingface.co/hf-inference` |

---

## 🔒 Security Practices

- API keys stored in `appsettings.json` (gitignored) — never hardcoded
- `appsettings-example.json` committed as a safe configuration template
- Strongly-typed options pattern (`IOptions<T>`) for all configuration
- `.gitignore` excludes `appsettings.json` and `appsettings.Development.json`

---

## 📈 Performance Characteristics

- **Batch embedding** — N entities = 1 HTTP call (not N calls)
- **Parallel vectorization** — 4 domains processed simultaneously
- **Parallel upsert** — 4 namespace writes run concurrently
- **Auto-chunking** — Large datasets split into 100-vector Pinecone batches
- **Async throughout** — Full `async/await` stack, no blocking calls

---

*Built with C# · .NET 10 · Hugging Face · Pinecone · MS SQL Server*
