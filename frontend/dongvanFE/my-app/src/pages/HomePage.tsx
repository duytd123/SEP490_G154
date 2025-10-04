import { Link } from "react-router-dom";
export default function HomePage() {
  return (
    <div className="flex flex-col">
      {" "}
      {/* Hero */}{" "}
      <section className="relative w-full h-[500px] flex items-center justify-center text-center text-white">
        {" "}
        <img
          src="/images/hero.jpg"
          alt="Hero background"
          className="absolute inset-0 w-full h-full object-cover"
        />{" "}
        <div className="absolute inset-0 bg-black/40" />{" "}
        <div className="relative z-10">
          {" "}
          <h2 className="text-4xl md:text-5xl font-extrabold mb-2">
            {" "}
            A COMPLETE TRAVEL THEME{" "}
          </h2>{" "}
          <p className="italic text-lg">Start your destiny here…</p>{" "}
        </div>{" "}
        {/* Search form */}{" "}
        <div className="absolute -bottom-12 w-11/12 md:w-3/4 lg:w-2/3 bg-white shadow-xl rounded p-4 flex flex-wrap gap-2 justify-center">
          {" "}
          {/* Tabs */}{" "}
          <div className="flex w-full gap-2 mb-2">
            {" "}
            <button className="flex-1 bg-gray-200 font-semibold py-2 rounded hover:bg-gray-300">
              Hotels
            </button>{" "}
            <button className="flex-1 bg-green-500 text-white font-semibold py-2 rounded">
              Packages
            </button>{" "}
            <button className="flex-1 bg-gray-200 font-semibold py-2 rounded hover:bg-gray-300">
              Places
            </button>{" "}
          </div>{" "}
          {/* Inputs */}{" "}
          <input
            placeholder="Type Hotel name here…"
            className="border px-3 py-2 w-full md:w-auto flex-1"
          />{" "}
          <select className="border px-3 py-2">
            <option>Choose City</option>
          </select>{" "}
          <select className="border px-3 py-2">
            <option>Choose Category</option>
          </select>{" "}
          <select className="border px-3 py-2">
            <option>Min Price</option>
          </select>{" "}
          <select className="border px-3 py-2">
            <option>Max Price</option>
          </select>{" "}
          <select className="border px-3 py-2">
            <option>Choose Offer</option>
          </select>{" "}
          <button className="bg-yellow-400 px-5 py-2 font-semibold">
            Find Hotels
          </button>{" "}
        </div>{" "}
      </section>{" "}
      {/* Packages */}{" "}
      <section className="mt-20 max-w-7xl mx-auto px-6 grid md:grid-cols-3 gap-8">
        {" "}
        {[
          { title: "ICE SKATING", price: 150, img: "/images/ice.jpg" },
          {
            title: "MOUNTAIN TREKKING",
            price: 120,
            img: "/images/mountain.jpg",
          },
          { title: "SURFING IN SEA", price: 250, img: "/images/sea.jpg" },
        ].map((p, i) => (
          <div
            key={i}
            className="border rounded shadow hover:shadow-lg transition"
          >
            {" "}
            <img
              src={p.img}
              alt={p.title}
              className="w-full h-48 object-cover rounded-t"
            />{" "}
            <div className="p-4">
              {" "}
              <h3 className="font-bold mb-2">{p.title}</h3>{" "}
              <p className="text-sm text-gray-600 mb-3">
                {" "}
                Lorem ipsum dolor sit amet, consectetur adipiscing elit…{" "}
              </p>{" "}
              <p className="font-bold">STARTS FROM ${p.price}</p>{" "}
              <button className="mt-2 px-4 py-2 bg-gray-700 text-white rounded hover:bg-gray-800">
                {" "}
                View details{" "}
              </button>{" "}
            </div>{" "}
          </div>
        ))}{" "}
      </section>{" "}
      {/* Welcome + Features */}{" "}
      <section className="py-20 bg-gray-50 mt-12">
        {" "}
        <div className="max-w-5xl mx-auto text-center">
          {" "}
          <h2 className="text-3xl font-bold text-sky-600">
            WELCOME TO TRENDY TRAVEL
          </h2>{" "}
          <p className="mt-4 text-gray-600">
            {" "}
            Sed ut perspiciatis unde omnis iste natus error sit voluptatem…{" "}
          </p>{" "}
          <div className="mt-6 flex justify-center gap-4">
            {" "}
            <button className="bg-green-500 text-white px-6 py-2 rounded">
              Browse Features
            </button>{" "}
            <button className="bg-sky-600 text-white px-6 py-2 rounded">
              Purchase Theme
            </button>{" "}
          </div>{" "}
        </div>{" "}
        {/* Features */}{" "}
        <div className="grid grid-cols-2 md:grid-cols-4 gap-6 mt-12 max-w-6xl mx-auto px-6">
          {" "}
          {[
            { title: "Special Activities", icon: "🎈" },
            { title: "Travel Arrangements", icon: "✈️" },
            { title: "Private Guide", icon: "🧭" },
            { title: "Location Manager", icon: "📍" },
          ].map((f, i) => (
            <div key={i} className="bg-white rounded shadow p-6 text-center">
              {" "}
              <div className="text-4xl mb-3">{f.icon}</div>{" "}
              <h4 className="font-bold text-sky-600">{f.title}</h4>{" "}
              <p className="text-gray-500 text-sm mt-2">
                Lorem ipsum dolor sit amet.
              </p>{" "}
            </div>
          ))}{" "}
        </div>{" "}
      </section>{" "}
    </div>
  );
}
