"use client"

export const Card = ({ children, className = "", ...props }: any) => (
  <div {...props} className={`rounded-md bg-card ${className}`}>
    {children}
  </div>
)

export const CardContent = ({ children, className = "", ...props }: any) => (
  <div {...props} className={`p-4 ${className}`}>
    {children}
  </div>
)

export const CardHeader = ({ children, className = "", ...props }: any) => (
  <div {...props} className={`p-4 border-b ${className}`}>
    {children}
  </div>
)

export const CardTitle = ({ children, className = "", ...props }: any) => (
  <h3 {...props} className={`text-lg font-semibold ${className}`}>
    {children}
  </h3>
)

export default Card
