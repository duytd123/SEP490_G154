"use client"

import { Card, CardContent } from "../ui/card"
import { Button } from "../ui/button"
import { Badge } from "../ui/badge"
import { Star, MapPin } from "../../lib/icons"

interface Hotel {
  id: number
  name: string
  image: string
  address: string
  price: number
  rating: number
  amenities: string[]
}

interface HotelCardProps {
  hotel: Hotel
}

export default function HotelCard({ hotel }: HotelCardProps) {
  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(price)
  }

  const handleBooking = () => {
    alert(`Đặt phòng tại: ${hotel.name}`)
    console.log("Booking hotel:", hotel.name)
  }

  const handleViewDetails = () => {
    alert(`Xem chi tiết: ${hotel.name}`)
    console.log("View details for hotel:", hotel.name)
  }

  return (
    <Card className="overflow-hidden hover:shadow-lg transition-all duration-300 hover:-translate-y-1">
      <CardContent className="p-0">
        <div className="flex flex-col md:flex-row">
          {/* Hotel Image */}
          <div className="md:w-1/3 relative h-48 md:h-auto">
            <img src={hotel.image || "/placeholder.svg"} alt={hotel.name} className="w-full h-full object-cover" />
            <div className="absolute top-3 right-3">
              <Badge variant="secondary" className="bg-card/90 text-foreground">
                <Star className="h-3 w-3 fill-yellow-400 text-yellow-400 mr-1" />
                {hotel.rating}
              </Badge>
            </div>
          </div>

          {/* Hotel Info */}
          <div className="md:w-2/3 p-6 flex flex-col justify-between">
            <div>
              <div className="flex items-start justify-between mb-2">
                <h3 className="text-xl font-semibold text-foreground hover:text-primary cursor-pointer">
                  {hotel.name}
                </h3>
                <div className="text-right">
                  <div className="text-2xl font-bold text-primary">{formatPrice(hotel.price)}</div>
                  <div className="text-sm text-muted-foreground">/ đêm</div>
                </div>
              </div>

              <div className="flex items-center text-muted-foreground mb-3">
                <MapPin className="h-4 w-4 mr-1" />
                <span className="text-sm">{hotel.address}</span>
              </div>

              {/* Amenities */}
              <div className="flex flex-wrap gap-2 mb-4">
                {hotel.amenities.slice(0, 4).map((amenity) => (
                  <Badge key={amenity} variant="outline" className="text-xs">
                    {amenity}
                  </Badge>
                ))}
                {hotel.amenities.length > 4 && (
                  <Badge variant="outline" className="text-xs">
                    +{hotel.amenities.length - 4} tiện nghi khác
                  </Badge>
                )}
              </div>
            </div>

            {/* Action Buttons */}
            <div className="flex gap-3 mt-4">
              <Button variant="outline" onClick={handleViewDetails} className="flex-1 bg-transparent">
                Xem chi tiết
              </Button>
              <Button onClick={handleBooking} className="flex-1 bg-primary text-primary-foreground hover:bg-primary/90">
                Đặt ngay
              </Button>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  )
}
