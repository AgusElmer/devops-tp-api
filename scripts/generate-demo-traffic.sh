#!/usr/bin/env bash
set -u

BASE_URL="${BASE_URL:-https://devops-tp-api.onrender.com}"

request() {
  local method="$1"
  local path="$2"
  local body="${3:-}"

  if [[ -n "$body" ]]; then
    curl -sS -o /tmp/demo-response.json -w "%{http_code} %{time_total}s ${method} ${path}\n" \
      -X "$method" "${BASE_URL}${path}" \
      -H "Content-Type: application/json" \
      -d "$body"
  else
    curl -sS -o /tmp/demo-response.json -w "%{http_code} %{time_total}s ${method} ${path}\n" \
      -X "$method" "${BASE_URL}${path}"
  fi
}

section() {
  printf "\n== %s ==\n" "$1"
}

section "Base URL"
echo "$BASE_URL"

section "General endpoints"
for path in /health /ready /version /diagnostics/ping; do
  request GET "$path"
done

section "Quest reads"
for i in {1..5}; do
  request GET /api/quests
done
request GET /api/quests/summary

section "Create quests"
created_ids=()
for i in {1..4}; do
  body="{\"title\":\"Presentation demo quest ${i}\",\"description\":\"Quest generated during the DevOps presentation demo.\",\"rank\":\"A\",\"type\":\"combat\",\"rewardGold\":$((100 + i)),\"rewardXp\":$((250 + i * 10))}"
  request POST /api/quests "$body"

  id="$(grep -o '"id":[0-9]*' /tmp/demo-response.json 2>/dev/null | head -n 1 | cut -d ':' -f 2 || true)"
  if [[ -n "${id:-}" ]]; then
    created_ids+=("$id")
  fi
done

section "Quest lifecycle"
count=0
for id in "${created_ids[@]}"; do
  count=$((count + 1))
  request GET "/api/quests/${id}"
  request PATCH "/api/quests/${id}/accept"

  if [[ "$count" -le 2 ]]; then
    request PATCH "/api/quests/${id}/complete"
  else
    request PATCH "/api/quests/${id}/abandon"
  fi
done

section "Slow requests for latency"
for i in {1..3}; do
  request GET /diagnostics/slow
done

section "Controlled errors for APM"
for i in {1..3}; do
  request GET /diagnostics/error
done

section "Final summary"
request GET /api/quests/summary

rm -f /tmp/demo-response.json

cat <<EOF

Traffic generated.
Wait 2-5 minutes and check New Relic:
- service: devops-tp-api
- throughput / requests
- response time
- errors
- traces
EOF
