"use client"

import HotelCard from "./hotel-card"

interface Hotel {
  id: number
  name: string
  image: string
  address: string
  price: number
  rating: number
  amenities: string[]
}

interface ResultListProps {
  hotels: Hotel[]
}

export default function ResultList({ hotels }: ResultListProps) {
  if (hotels.length === 0) {
    return (
      <div className="text-center py-12">
        <div className="text-6xl mb-4">🏨</div>
        <h3 className="text-xl font-semibold text-foreground mb-2">Không tìm thấy khách sạn phù hợp</h3>
        <p className="text-muted-foreground">Hãy thử điều chỉnh bộ lọc hoặc từ khóa tìm kiếm</p>
      </div>
    )
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-xl font-semibold text-foreground">Tìm thấy {hotels.length} khách sạn</h2>
        <select className="px-3 py-2 border border-border rounded-md text-sm bg-card">
          <option>Sắp xếp theo: Phù hợp nhất</option>
          <option>Giá: Thấp đến cao</option>
          <option>Giá: Cao đến thấp</option>
          <option>Đánh giá cao nhất</option>
        </select>
      </div>

      <div className="grid gap-6">
        {hotels.map((hotel) => (
          <HotelCard key={hotel.id} hotel={hotel} />
        ))}
      </div>
    </div>
  )
}
