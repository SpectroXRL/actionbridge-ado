# ActionBridge ADO

> A reliable pipeline for turning transcripts and documents into Azure DevOps work items using AI.

ActionBridge ADO is a full-stack application that automatically converts documents (text, Word docs, transcripts) into structured Azure DevOps work items using Azure OpenAI. Simply upload a document, let AI parse it into actionable tasks, review the generated work items, and create them directly in your Azure DevOps project.

## 🌟 Features

- **AI-Powered Parsing**: Leverages Azure OpenAI to intelligently extract work items from unstructured documents
- **Document Upload**: Support for multiple file formats (.txt, .doc, .docx, .xml)
- **Work Item Generation**: Automatically creates Azure DevOps work items with:
  - Title and description
  - Work item type (Task, Epic, Issue)
  - Priority levels (1-4)
  - Tags
- **Interactive Review**: Review and edit AI-generated work items before creating them
- **Azure DevOps Integration**: Direct integration with Azure DevOps API for seamless work item creation
- **Modern UI**: React-based frontend with TypeScript for type safety
- **RESTful API**: Clean, documented API endpoints

## 🏗️ Architecture

The project consists of two main components:

### Backend (ActionBridge-Ado.Api)
- **Technology**: .NET 9.0 Web API
- **Key Dependencies**:
  - Azure.AI.OpenAI (2.1.0) - AI parsing capabilities
  - Microsoft.TeamFoundationServer.Client (19.225.2) - Azure DevOps integration
  - Microsoft.Identity.Client (4.82.0) - Authentication
- **Services**:
  - `AIService` - Handles document parsing and AI work item generation
  - `AdoService` - Manages Azure DevOps API interactions
  - `AuthService` - Handles authentication with Microsoft Entra ID

### Frontend (ActionBridge-Ado.Client)
- **Technology**: React 19 + TypeScript + Vite
- **Features**:
  - File upload interface
  - Work item review and editing
  - Real-time status updates
  - Responsive design

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js](https://nodejs.org/) (v18 or higher)
- [Azure OpenAI Service](https://azure.microsoft.com/products/ai-services/openai-service) access
- [Azure DevOps](https://azure.microsoft.com/products/devops/) organization and project
- Microsoft Entra ID (Azure AD) application registration

## 🚀 Installation

### 1. Clone the Repository

```
git clone https://github.com/SpectroXRL/actionbridge-ado.git
cd actionbridge-ado
```

### 2. Backend Setup

Navigate to the API project:

```
cd ActionBridge-Ado.Api
```

Restore .NET dependencies:

```
dotnet restore
```

### 3. Frontend Setup

Navigate to the client project:

```
cd ../ActionBridge-Ado.Client
```

Install npm dependencies:

```
npm install
```

## ⚙️ Configuration

### Backend Configuration

#### 1. Azure OpenAI Settings

Add your Azure OpenAI credentials to \`appsettings.Development.json\` or use User Secrets:

```
cd ActionBridge-Ado.Api
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://your-resource.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "your-api-key"
dotnet user-secrets set "AzureOpenAI:DeploymentName" "your-deployment-name"
```

Or add to `appsettings.Development.json`:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "your-api-key",
    "DeploymentName": "your-deployment-name"
  }
}
```

#### 2. Microsoft Entra ID Configuration

Configure authentication for Azure DevOps API access. You'll need to register an application in Microsoft Entra ID and configure the appropriate scopes.

### Frontend Configuration

Create a `.env` file in the \`ActionBridge-Ado.Client\` directory:

```env
VITE_ORGANIZATION=your-organization
VITE_PROJECT=your-project-name
VITE_ORGANIZATION_URL=https://dev.azure.com/your-organization
```

## 🎯 Usage

### Starting the Application

#### 1. Start the Backend API

```bash
cd ActionBridge-Ado.Api
dotnet run
```

The API will start on `http://localhost:5277`

#### 2. Start the Frontend

In a new terminal:

```bash
cd ActionBridge-Ado.Client
npm run dev
```

The frontend will start on `http://localhost:5173`

### Using the Application

1. **Open your browser** and navigate to `http://localhost:5173`
2. **Upload a document** containing work items (transcript, notes, requirements doc)
3. **Review AI-generated work items** - the AI will parse your document and extract:
   - Titles
   - Descriptions
   - Work item types
   - Priorities
   - Tags
4. **Edit if needed** - modify any generated work items before creation
5. **Create in Azure DevOps** - click the submit button to create all work items in your ADO project

### Example Document Format

Your input document can be unstructured text. The AI will intelligently parse it:

```
Meeting Notes - Sprint Planning

We need to implement user authentication:
- Create login page with email/password
- Add password reset functionality
- Implement JWT token authentication

High priority: Fix the bug where users can't submit forms on mobile devices

Also need to add a new feature for exporting reports to PDF
```

The AI will generate appropriate work items with titles, descriptions, types, and priorities.

## 📚 API Documentation

### Endpoints

#### File Upload
```
POST /api/file/upload
Query Parameters:
  - organization: string
  - project: string
Body: multipart/form-data with file
Response: Array of WorkItemRequest objects
```

#### Get Projects
```
GET /api/ado/projects
Query Parameters:
  - organizationUrl: string
Response: Array of project objects
```

#### Create Work Items
```
POST /api/ado/workitems
Query Parameters:
  - organizationUrl: string
  - project: string
Body: Array of WorkItemRequest objects
Response: Created work items with IDs and URLs
```

### Work Item Request Schema

```
{
  "Title": "string",
  "Description": "string",
  "Type": "Task | Epic | Issue",
  "Tags": "string (comma-separated)",
  "Priority": 1 | 2 | 3 | 4,
  "AssignedTo": "string (optional)"
}
```

## 🛠️ Development

### Backend Development

Build the API:
```bash
cd ActionBridge-Ado.Api
dotnet build
```

Run tests (if available):
```
dotnet test
```

### Frontend Development

Lint the code:
```
cd ActionBridge-Ado.Client
npm run lint
```

Build for production:
```
npm run build
```

Preview production build:
```
npm run preview
```

### Project Structure

```
actionbridge-ado/
├── ActionBridge-Ado.Api/          # .NET Web API
│   ├── Endpoints/                 # API endpoint definitions
│   │   ├── FileEndpoints.cs      # File upload endpoints
│   │   └── AdoEndpoints.cs       # Azure DevOps endpoints
│   ├── Models/                    # Data models
│   │   ├── WorkItemRequest.cs
│   │   ├── WorkItemResponse.cs
│   │   └── AzureOpenAIOptions.cs
│   ├── Services/                  # Business logic
│   │   ├── AI/                    # AI service for parsing
│   │   ├── Ado/                   # Azure DevOps integration
│   │   └── Auth/                  # Authentication service
│   ├── Program.cs                 # App entry point
│   └── appsettings.json          # Configuration
├── ActionBridge-Ado.Client/       # React frontend
│   ├── src/
│   │   ├── components/           # React components
│   │   ├── types/                # TypeScript types
│   │   ├── App.tsx              # Main app component
│   │   └── main.tsx             # App entry point
│   ├── package.json
│   └── vite.config.ts
└── README.md
```

## 🔒 Security Considerations

- **API Keys**: Never commit Azure OpenAI API keys to version control. Use User Secrets or environment variables.
- **Authentication**: The application uses Microsoft Entra ID for Azure DevOps authentication.
- **CORS**: The API is configured to allow requests from the React app (localhost:5173 by default).
- **Input Validation**: File uploads are validated and sanitized.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

### Development Guidelines

1. Follow the existing code style
2. Write meaningful commit messages
3. Update documentation as needed
4. Ensure all tests pass before submitting PR

## 📝 License

This project is provided as-is for demonstration and development purposes.

## 🙏 Acknowledgments

- Built with [Azure OpenAI](https://azure.microsoft.com/products/ai-services/openai-service)
- Integrated with [Azure DevOps](https://azure.microsoft.com/products/devops/)
- Frontend powered by [React](https://react.dev/) and [Vite](https://vite.dev/)
- Backend powered by [.NET](https://dotnet.microsoft.com/)

## 📞 Support

For issues, questions, or contributions, please use the GitHub Issues page.

---

**Note**: This application requires active Azure OpenAI and Azure DevOps subscriptions. Ensure you have the necessary permissions and quotas before deployment.
