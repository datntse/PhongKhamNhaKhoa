echo "Start building clinic-be";
docker build -t clinic-be .;

echo "Starting up clinic-be";
docker run -d -p 7210:80 --name clinic-be clinic-be