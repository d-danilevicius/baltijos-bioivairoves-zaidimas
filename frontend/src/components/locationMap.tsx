import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import { useEffect, useRef } from 'react'
import { lithuaniaCenter } from '../helpers/lithuaniaBounds'
import { mapLabel } from '../helpers/gbifTaskMessage'

type Props = {
  latitude?: number | undefined
  longitude?: number | undefined
  onSelect: (latitude: number, longitude: number) => void
}

const initialZoom = 7
const selectedZoom = 11
const attribution = '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'

// The map is only used to choose a point; the point can also be typed as coordinates
export default function LocationMap({ latitude, longitude, onSelect }: Props) {
  const containerRef = useRef<HTMLDivElement>(null)
  const mapRef = useRef<L.Map | null>(null)
  const markerRef = useRef<L.CircleMarker | null>(null)
  const onSelectRef = useRef(onSelect)

  useEffect(() => {
    onSelectRef.current = onSelect
  })

  useEffect(() => {
    const container = containerRef.current
    if (!container) return

    const map = L.map(container).setView([lithuaniaCenter.latitude, lithuaniaCenter.longitude], initialZoom)
    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 19, attribution }).addTo(map)
    map.on('click', (event: L.LeafletMouseEvent) => onSelectRef.current(event.latlng.lat, event.latlng.lng))
    mapRef.current = map

    return () => {
      map.remove()
      mapRef.current = null
      markerRef.current = null
    }
  }, [])

  useEffect(() => {
    const map = mapRef.current
    if (!map || latitude === undefined || longitude === undefined) return

    if (markerRef.current) {
      markerRef.current.setLatLng([latitude, longitude])
    } else {
      markerRef.current = L.circleMarker([latitude, longitude], { radius: 9 }).addTo(map)
    }
    map.setView([latitude, longitude], Math.max(map.getZoom(), selectedZoom))
  }, [latitude, longitude])

  return <div ref={containerRef} role="application" aria-label={mapLabel} className="mb-4 h-72 w-full rounded-md" />
}
