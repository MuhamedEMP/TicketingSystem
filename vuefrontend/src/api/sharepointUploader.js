const GRAPH_ENDPOINT = "https://graph.microsoft.com/v1.0";
const SITE_HOST = "emediapatchcom.sharepoint.com"; // your tenant
const SITE_NAME = "ticketing";                // your site name
const LIBRARY_FOLDER = "Shared Documents/Uploads"; // folder in document library

export async function uploadToSharePoint(file) {
  const graphToken = localStorage.getItem("graphAccessToken");

  if (!graphToken) {
    throw new Error("❌ Microsoft Graph access token not found.");
  }

  // 🔍 Get the SharePoint site ID
  const siteRes = await fetch(
    `${GRAPH_ENDPOINT}/sites/${SITE_HOST}:/sites/${SITE_NAME}`,
    {
      headers: {
        Authorization: `Bearer ${graphToken}`,
      },
    }
  );

  if (!siteRes.ok) {
    const errText = await siteRes.text();
    throw new Error(`❌ Failed to fetch site info: ${errText}`);
  }

  const site = await siteRes.json();

  // 📤 Upload file to SharePoint document library
  const uploadRes = await fetch(
    `${GRAPH_ENDPOINT}/sites/${site.id}/drive/root:/${LIBRARY_FOLDER}/${file.name}:/content`,
    {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${graphToken}`,
        "Content-Type": file.type,
      },
      body: file,
    }
  );

  if (!uploadRes.ok) {
    const errText = await uploadRes.text();
    throw new Error(`❌ Upload failed: ${errText}`);
  }

  const uploadedFile = await uploadRes.json();
  console.log("✅ Uploaded to SharePoint:", uploadedFile);
  return uploadedFile.webUrl;
}
