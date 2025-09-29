"use client"

import { useState } from "react"
import Header from "../../components/homestay/header"
import SearchBar from "../../components/homestay/search-bar"
import FilterPanel from "../../components/homestay/filter-panel"
import ResultList from "../../components/homestay/result-list"
import Footer from "../../components/homestay/footer"

// Mock data for hotels
const mockHotels = [
  {
    id: 1,
    name: "Grand Hotel Saigon",
    image: "/luxury-hotel-exterior.png",
    address: "Quận 1, TP. Hồ Chí Minh",
    price: 2500000,
    rating: 4.8,
    amenities: ["WiFi", "Pool", "Gym", "Restaurant"],
  },
  {
    id: 2,
    name: "Riverside Resort Da Nang",
    image: "/beachfront-resort.jpg",
    address: "Đà Nẵng",
    price: 1800000,
    rating: 4.6,
    amenities: ["Beach Access", "Spa", "WiFi", "Pool"],
  },
  {
    id: 3,
    name: "Hanoi Heritage Hotel",
    image: "/boutique-hotel.png",
    address: "Hoàn Kiếm, Hà Nội",
    price: 1200000,
    rating: 4.4,
    amenities: ["WiFi", "Restaurant", "Bar", "Concierge"],
  },
  {
    id: 4,
    name: "Phu Quoc Beach Resort",
    image: "/tropical-beach-resort.png",
    address: "Phú Quốc, Kiên Giang",
    price: 3200000,
    rating: 4.9,
    amenities: ["Private Beach", "Spa", "Pool", "Water Sports"],
  },
  {
    id: 5,
    name: "Sapa Mountain Lodge",
    image: "/cozy-mountain-lodge.png",
    address: "Sa Pa, Lào Cai",
    price: 900000,
    rating: 4.2,
    amenities: ["Mountain View", "Fireplace", "Restaurant", "Hiking"],
  },
  {
    id: 6,
    name: "Hoi An Ancient House",
    image: "/traditional-vietnamese-house.png",
    address: "Hội An, Quảng Nam",
    price: 1500000,
    rating: 4.7,
    amenities: ["Historic Building", "Garden", "WiFi", "Bicycle Rental"],
  },
]

type Filters = {
  priceRange: [number, number]
  rating: number
  amenities: string[]
}

export default function HomePage() {
  const [hotels, setHotels] = useState(mockHotels)
  const [searchQuery, setSearchQuery] = useState("")
  const [filters, setFilters] = useState<Filters>({
    priceRange: [0, 5000000],
    rating: 0,
    amenities: [],
  })

  const handleSearch = (query: string, _checkIn: Date | null, _checkOut: Date | null, _guests: number) => {
    setSearchQuery(query)
    // Filter hotels based on search query
    if (query.trim()) {
      const filtered = mockHotels.filter(
        (hotel) =>
          hotel.name.toLowerCase().includes(query.toLowerCase()) ||
          hotel.address.toLowerCase().includes(query.toLowerCase()),
      )
      setHotels(filtered)
    } else {
      setHotels(mockHotels)
    }
  }

  const handleFilterChange = (newFilters: Filters) => {
    setFilters(newFilters)
    // Apply filters to hotels
    let filtered = searchQuery.trim()
      ? mockHotels.filter(
          (hotel) =>
            hotel.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
            hotel.address.toLowerCase().includes(searchQuery.toLowerCase()),
        )
      : mockHotels

    filtered = filtered.filter(
      (hotel) =>
        hotel.price >= newFilters.priceRange[0] &&
        hotel.price <= newFilters.priceRange[1] &&
        hotel.rating >= newFilters.rating,
    )

    if (newFilters.amenities.length > 0) {
      filtered = filtered.filter((hotel) => newFilters.amenities.some((amenity) => hotel.amenities.includes(amenity)))
    }

    setHotels(filtered)
  }

  return (
    <div className="min-h-screen bg-background">
      <Header />
      <main className="w-full px-4 py-8">
        <div className="max-w-screen-lg mx-auto">
          <SearchBar onSearch={handleSearch} />
        </div>

        <div className="flex flex-col lg:flex-row gap-8 mt-8 w-full">
          <aside className="lg:w-1/4 w-full">
            <div className="h-full">
              <FilterPanel filters={filters} onFilterChange={handleFilterChange} />
            </div>
          </aside>
          <section className="lg:flex-1 w-full">
            <div className="w-full">
              <ResultList hotels={hotels} />
            </div>
          </section>
        </div>
      </main>
      <Footer />
    </div>
  )
}
