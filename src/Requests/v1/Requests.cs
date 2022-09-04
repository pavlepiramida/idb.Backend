using System;
using System.Collections.Generic;

namespace idb.Backend.Requests.v1
{
    public record TagsResponse(int id, string name);

    public record ItemResponse(int id, string guid, string name, List<TagsResponse> tags, string content,
        string content_html, DateTime? created_at);

    public record ItemPostRequest(string name, string content, List<int> tag_ids);

    public record ItemsPatchRequest(string content, List<int> tag_ids);

    public record LoginRequest(string Email, string Password);

    public record UserResponse(int id, string guid, string email, string first_name, string last_name,
        DateTime joined_at, bool is_admin);

    public record TokenResponse(string token);

    public record ImageUpload(string filename, string content_type);

    public record ImageUploadResponse(string signed_url, string upload_to);
}