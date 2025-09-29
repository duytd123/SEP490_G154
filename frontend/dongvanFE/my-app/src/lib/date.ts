export const formatDate = (date: Date, formatStr = "dd/MM/yyyy") => {
  const dd = String(date.getDate()).padStart(2, "0")
  const mm = String(date.getMonth() + 1).padStart(2, "0")
  const yyyy = String(date.getFullYear())
  return formatStr.replace("dd", dd).replace("MM", mm).replace("yyyy", yyyy)
}

export const vi = {} // placeholder locale

export default {}
