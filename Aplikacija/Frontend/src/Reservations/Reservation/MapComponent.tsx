import { MapContainer, TileLayer, Marker } from "react-leaflet";
import { LatLng } from "leaflet";

function MapComponent() {
  const position = new LatLng(51.505, -0.09);

  return (
    <MapContainer
      center={position}
      zoom={13}
      scrollWheelZoom={false}
      style={{ width: "100%", height: "auto", flex: 1 }}
    >
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />
      <Marker position={position}></Marker>
    </MapContainer>
  );
}

export default MapComponent;
