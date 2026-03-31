import http from "k6/http";
import { sleep } from "k6";

export default function () {
  const url = "http://dev-allen.demoapi.com/api/ticket/order";
  const payload = JSON.stringify({
    userId: `user_${__ITER}`,
    eventId: "concert_2026",
    quantity: Math.floor(Math.random() * 10) + 1,
    requestTime: new Date().toISOString(),
  });

  const params = { headers: { "Content-Type": "application/json" } };
  http.post(url, payload, params);
}
